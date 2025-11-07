(function () {
    const splash = document.getElementById('splash');
    const gate = document.getElementById('gate');
    const input = document.getElementById('codeInput');
    const STORAGE_KEY = 'code_gate:lastCode';

    window.addEventListener('load', () => {
        setTimeout(() => {
            splash?.classList.add('hidden');
            gate?.classList.remove('hidden');
            input?.focus();
            const last = localStorage.getItem(STORAGE_KEY);
            if (last && input) input.value = last;
        }, 2000);

        // Fallback de secours au cas où le premier ne passe pas
        setTimeout(() => {
            if (!gate || !splash) return;
            gate.classList.remove('hidden');
            splash.classList.add('hidden');
        }, 3000);
    });


    if (input) {
        input.addEventListener('input', (e) => {
            const el = e.target;
            if (!(el instanceof HTMLInputElement)) return;
            const raw = el.value.toUpperCase().replace(/[^A-Z0-9-]/g, '');
            const noDash = raw.replace(/-/g, '');
            const letters = (noDash.slice(0, 2).match(/[A-Z]*/)?.[0] || '').toUpperCase();
            const digits = (noDash.slice(2, 4).match(/\d*/)?.[0] || '');
            const formatted = letters + (noDash.length > 2 ? '-' : '') + digits;
            el.value = formatted.slice(0, 5);
        });

        const form = input.closest('form');
        form?.addEventListener('submit', () => {
            try { localStorage.setItem(STORAGE_KEY, input.value.toUpperCase()); } catch { }
        });
    }
})();
console.log("gate.js chargé");

