// wwwroot/js/orders-create.js
document.addEventListener('DOMContentLoaded', () => {
    console.log('[OrdersCreate] init');

    // --- éléments requis ---
    const taxRateEl = document.getElementById('taxRateText');
    const btnChangeClient = document.getElementById('btnChangeClient');
    const selectedClientName = document.getElementById('selectedClientName');
    const clientModalEl = document.getElementById('clientModal');
    const clientSearch = document.getElementById('clientSearch');
    const clientResults = document.getElementById('clientResults');
    const productSearch = document.getElementById('productSearch');
    const productResults = document.getElementById('productResults');
    const itemsTable = document.getElementById('itemsTable');
    const couponInput = document.getElementById('couponInput');
    const applyCoupon = document.getElementById('applyCoupon');
    const couponMsg = document.getElementById('couponMsg');
    const subtotalText = document.getElementById('subtotalText');
    const discountText = document.getElementById('discountText');
    const taxText = document.getElementById('taxText');
    const totalText = document.getElementById('totalText');
    const saveOrder = document.getElementById('saveOrder');

    if (!btnChangeClient || !productSearch || !itemsTable) {
        console.error('[OrdersCreate] éléments introuvables. Vérifie les IDs dans la vue.');
        return;
    }

    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
    const taxRate = Number((taxRateEl?.innerText || '15').replace('%', '')) / 100 || 0.15;

    // --- état ---
    let items = []; // {productId, sku, name, unitPrice, quantity}
    let client = { id: null, name: 'STORE CUSTOMER' };
    let discount = 0;
    let couponCode = null;

    // --- helpers ---
    const fmt = v => (v ?? 0).toLocaleString(undefined, { style: 'currency', currency: 'USD' });
    const calcSubtotal = () => items.reduce((s, i) => s + i.unitPrice * i.quantity, 0);
    const renderTotals = () => {
        const st = +(calcSubtotal().toFixed(2));
        const tx = +(((st - discount) * taxRate)).toFixed(2);
        const tot = Math.max(0, +(st - discount + tx).toFixed(2));
        subtotalText.innerText = fmt(st);
        discountText.innerText = fmt(discount);
        taxText.innerText = fmt(tx);
        totalText.innerText = fmt(tot);
    };
    const refreshTable = () => {
        const tbody = itemsTable.querySelector('tbody');
        tbody.innerHTML = '';
        items.forEach((it, idx) => {
            const tr = document.createElement('tr');
            tr.innerHTML = `
        <td>${it.sku}</td>
        <td>${it.name}</td>
        <td class="text-end">${fmt(it.unitPrice)}</td>
        <td class="text-end">
          <input type="number" min="1" class="form-control form-control-sm text-end qtyInp" data-idx="${idx}" value="${it.quantity}">
        </td>
        <td class="text-end">${fmt(it.unitPrice * it.quantity)}</td>
        <td class="text-end"><button class="btn btn-sm btn-outline-danger rmBtn" data-idx="${idx}">X</button></td>
      `;
            tbody.appendChild(tr);
        });
        renderTotals();
    };

    // --- modal client (fallback si Bootstrap absent) ---
    let clientModal = null;
    const openClientModal = () => {
        if (window.bootstrap && clientModalEl) {
            clientModal ??= new bootstrap.Modal(clientModalEl);
            clientModal.show();
        } else {
            // fallback basique : on affiche le bloc
            clientModalEl?.classList.add('show');
            clientModalEl?.style.setProperty('display', 'block');
        }
    };
    const closeClientModal = () => {
        if (window.bootstrap && clientModal) clientModal.hide();
        else {
            clientModalEl?.classList.remove('show');
            clientModalEl?.style.removeProperty('display');
        }
    };

    btnChangeClient.addEventListener('click', () => {
        if (clientSearch) clientSearch.value = '';
        if (clientResults) clientResults.innerHTML = '';
        openClientModal();
    });

    clientSearch?.addEventListener('input', async (e) => {
        const q = e.target.value;
        try {
            const res = await fetch(`/Orders/SearchClients?q=${encodeURIComponent(q)}`);
            const data = await res.json();
            clientResults.innerHTML = '';
            data.forEach(c => {
                const li = document.createElement('li');
                li.className = 'list-group-item list-group-item-action';
                li.textContent = `${c.name} — ${c.email ?? ''}`;
                li.onclick = () => {
                    client = { id: c.id, name: c.name };
                    selectedClientName.innerText = client.name;
                    closeClientModal();
                };
                clientResults.appendChild(li);
            });
        } catch (err) {
            console.error('[OrdersCreate] SearchClients error', err);
        }
    });

    // --- recherche produits ---
    let searchTimeout;
    productSearch.addEventListener('input', (e) => {
        clearTimeout(searchTimeout);
        const q = e.target.value;
        searchTimeout = setTimeout(async () => {
            if (!q || q.length < 2) { productResults.innerHTML = ''; return; }
            try {
                const res = await fetch(`/Orders/SearchProducts?q=${encodeURIComponent(q)}`);
                const data = await res.json();
                productResults.innerHTML = '';
                data.forEach(p => {
                    const a = document.createElement('a');
                    a.className = 'list-group-item list-group-item-action d-flex justify-content-between align-items-center';
                    a.innerHTML = `<div><div class="fw-bold">${p.name}</div><div class="small text-muted">${p.sku} — Stock: ${p.stock}</div></div><div>${fmt(p.price)}</div>`;
                    a.onclick = () => addProductPrompt(p);
                    productResults.appendChild(a);
                });
            } catch (err) {
                console.error('[OrdersCreate] SearchProducts error', err);
            }
        }, 250);
    });

    async function addProductPrompt(p) {
        const qty = parseInt(prompt(`Quantité pour "${p.name}" ? (Stock dispo: ${p.stock})`, "1"), 10);
        if (isNaN(qty) || qty <= 0) return;
        if (qty > p.stock) { alert(`Stock insuffisant. Disponible: ${p.stock}`); return; }
        const existing = items.find(i => i.productId === p.id);
        if (existing) {
            const newQty = existing.quantity + qty;
            if (newQty > p.stock) { alert(`Stock insuffisant. Disponible: ${p.stock}`); return; }
            existing.quantity = newQty;
        } else {
            items.push({ productId: p.id, sku: p.sku, name: p.name, unitPrice: p.price, quantity: qty });
        }
        productResults.innerHTML = '';
        productSearch.value = '';
        refreshTable();
    }

    // table: qty / remove
    itemsTable.addEventListener('input', (e) => {
        if (e.target.classList.contains('qtyInp')) {
            const idx = +e.target.dataset.idx; const v = +e.target.value;
            if (!Number.isFinite(v) || v <= 0) { e.target.value = items[idx].quantity; return; }
            items[idx].quantity = v;
            refreshTable();
        }
    });
    itemsTable.addEventListener('click', (e) => {
        if (e.target.classList.contains('rmBtn')) {
            const idx = +e.target.dataset.idx;
            items.splice(idx, 1);
            refreshTable();
        }
    });

    // coupon
    applyCoupon?.addEventListener('click', async () => {
        const code = (couponInput.value || '').trim();
        if (!code) { couponMsg.innerText = 'Entrez un code.'; return; }
        try {
            const fd = new FormData();
            fd.append('code', code);
            fd.append('subtotal', calcSubtotal());
            const res = await fetch('/Orders/ValidateCoupon', { method: 'POST', body: fd });
            const data = await res.json();
            if (!data.ok) { discount = 0; couponCode = null; couponMsg.innerText = data.message; }
            else { discount = data.discount; couponCode = code; couponMsg.innerText = `Coupon appliqué: -${fmt(discount)}`; }
            renderTotals();
        } catch (err) {
            console.error('[OrdersCreate] ValidateCoupon error', err);
        }
    });

    // save
    saveOrder.addEventListener('click', async () => {
        if (items.length === 0) { alert('Ajoutez au moins un article.'); return; }
        try {
            const res = await fetch('/Orders/CreateOrder', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token
                },
                body: JSON.stringify({
                    clientId: client.id,
                    clientName: client.name,
                    items,
                    couponCode,
                    discountAmount: discount,
                    taxRate
                })
            });
            if (!res.ok) {
                const txt = await res.text();
                alert('Erreur: ' + txt);
                return;
            }
            const data = await res.json();
            if (data.ok) window.location.href = `/Orders/Details/${data.id}`;
        } catch (err) {
            console.error('[OrdersCreate] CreateOrder error', err);
        }
    });

    // init
    renderTotals();
});
