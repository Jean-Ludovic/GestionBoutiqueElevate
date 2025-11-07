using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GestionBoutiqueElevate.Controllers
{
    public class MailController : Controller
    {
        // GET: MailController
        public ActionResult Index()
        {
            return View();
        }

        // GET: MailController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: MailController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MailController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: MailController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: MailController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: MailController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: MailController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
