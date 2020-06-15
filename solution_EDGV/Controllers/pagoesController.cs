using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using solution_EDGV.Models;

namespace solution_EDGV.Controllers
{
    public class pagoesController : Controller
    {
        private BD_EDGVEntities db = new BD_EDGVEntities();

        // GET: pagoes
        public ActionResult Index()
        {
            var pagoes = db.pagoes.Include(p => p.factura);
            return View(pagoes.ToList());
        }

     


        // GET: pagoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            pago pago = db.pagoes.Find(id);
            if (pago == null)
            {
                return HttpNotFound();
            }
            return View(pago);
        }

        // GET: pagoes/Create
        public ActionResult Create()
        {
            ViewBag.idfactura = new SelectList(db.facturas, "id", "id");
            return View();
        }

        // POST: pagoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,fecha,idfactura,montopago")] pago pago)
        {
            string ms="";
            //Buscar Factura 

            if (pago.montopago <= 0) {
                    ms ="Monto de Pago debe ser mayor a un dolar";
            }
            
            factura factura = db.facturas.Find(pago.idfactura); 
            if (factura.saldo - pago.montopago < 0)
            {
                ms = "No puede sobre pasar el saldo  de la factura";
            
            }
            else
            {
                factura.saldo = factura.saldo - pago.montopago; 
                db.Entry(factura).State = EntityState.Modified;
                db.SaveChanges();

                if (ModelState.IsValid)
                {
                    db.pagoes.Add(pago);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

               
            }
            if(ms != "") {

                ViewBag.Message = ms;
            }
            ViewBag.idfactura = new SelectList(db.facturas, "id", "id", pago.idfactura);

            return View(pago);
        }

        // GET: pagoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            pago pago = db.pagoes.Find(id);
            if (pago == null)
            {
                return HttpNotFound();
            }
            ViewBag.idfactura = new SelectList(db.facturas, "id", "id", pago.idfactura);
            return View(pago);
        }

        // POST: pagoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,fecha,idfactura,montopago")] pago pago)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pago).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idfactura = new SelectList(db.facturas, "id", "id", pago.idfactura);
            return View(pago);
        }

        // GET: pagoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            pago pago = db.pagoes.Find(id);
            if (pago == null)
            {
                return HttpNotFound();
            }
            return View(pago);
        }

        // POST: pagoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            pago pago = db.pagoes.Find(id);
           
            /*restituye monto*/
            factura factura = db.facturas.Find(pago.idfactura);
            factura.saldo = factura.saldo + pago.montopago;
            db.Entry(factura).State = EntityState.Modified;

            db.pagoes.Remove(pago);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
