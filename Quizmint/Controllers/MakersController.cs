using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Quizmint.Models;
using System.Configuration;
using System.Web.Security;
using System.Net.Mail;

namespace Quizmint.Controllers
{
    public class MakersController : Controller
    {
        private ShamuEntities db = new ShamuEntities();

        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration([Bind(Exclude = "IsEmailVerified, ActivationCode")] Maker maker)
        {
            bool statusOK = false;
            string message = null;

            //Model validation
            if (ModelState.IsValid)
            {
                //Email already exist
                if (IsEmailExist(maker.Email))
                {
                    ModelState.AddModelError("EmailExist", "Email is already exist");
                    return View(maker);
                }

                //Generate activation code
                maker.ActivationCode = Guid.NewGuid();

                //Password hashing
                maker.Password = Encrypt.Hash(maker.Password);
                maker.ConfirmPassword = Encrypt.Hash(maker.ConfirmPassword);

                //Create now
                maker.RegisteredDate = DateTime.Now;

                //Save to database
                db.Makers.Add(maker);
                db.SaveChanges();

                //Send email to user
                SendVerificationEmail(maker.Email, maker.ActivationCode.ToString());
                message = "Registration is completed. Please check you email to confirm : " + maker.Email;
                statusOK = true;
            }
            else
            {
                message = "Improper model";
            }

            ViewBag.Message = message;
            ViewBag.StatusOK = statusOK;
            return View(maker);
        }

        public ActionResult VerifyAccount(string id)
        {
            //Verify account
            string message = null;
            try
            {
                var maker = db.Makers.Where(a => a.ActivationCode == new Guid(id)).FirstOrDefault();
                if (maker != null)
                {
                    maker.IsEmailVerified = true;
                    db.Configuration.ValidateOnSaveEnabled = false; //skip model validation
                    db.SaveChanges();
                }
                else
                {
                    message = "Invalid request";
                }
            }
            catch (Exception e)
            {
                //most likely invalid Guid
                message = e.Message;
            }

            ViewBag.Message = message;
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Maker model, string returnUrl = "")
        {
            string message = null;
            var maker = db.Makers.Where(a => a.Email == model.Email).FirstOrDefault();
            if (maker != null)
            {
                if (maker.IsEmailVerified)
                {
                    if (string.Compare(Encrypt.Hash(model.Password), maker.Password) == 0)
                    {
                        int timeout = model.RememberMe ? 525600 : 20; // 1 yr
                        var ticket = new FormsAuthenticationTicket(model.Email, model.RememberMe, timeout);
                        string encrypted = FormsAuthentication.Encrypt(ticket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                        cookie.Expires = DateTime.Now.AddMinutes(timeout);
                        cookie.HttpOnly = true;
                        Response.Cookies.Add(cookie);

                        Session["MakerId"] = maker.Id;

                        if (Url.IsLocalUrl(returnUrl))
                        {
                            //redirect to [Authorized] view where it was initially intended 
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        message = "Incorrect password provided";
                    }
                }
                else
                {
                    message = "Please verify you email first before you can log in : " + maker.Email;
                }
            }
            else
            {
                message = "Email is not registered";
            }

            ViewBag.Message = message;
            return View();
        }

        [Authorize]
        public ActionResult Logout()
        {
            Session.RemoveAll(); 
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(Maker model)
        {
            string message = null;
            bool statusOK = false;

            var maker = db.Makers.Where(a => a.Email == model.Email).FirstOrDefault();
            if (maker != null)
            {
                db.Configuration.ValidateOnSaveEnabled = false; //skip model validation
                maker.ResetPasswordCode = Guid.NewGuid();
                db.SaveChanges();

                //Send email for reset password
                SendForgotPasswordEmail(maker.Email, maker.ResetPasswordCode.ToString());
                message = "Reset password link has been sent to your email.";
                statusOK = true;
            }
            else
            {
                message = "Account not found";
            }

            ViewBag.Message = message;
            ViewBag.StatusOK = statusOK;
            return View();
        }

        public ActionResult VerifyResetPassword(string id)
        {
            string message = null;

            try
            {
                var maker = db.Makers.Where(a => a.ResetPasswordCode == new Guid(id)).FirstOrDefault();
                if (maker != null)
                {
                    Maker model = new Maker();
                    model.ResetPasswordCode = new Guid(id);
                    return View(model);
                }
                else
                {
                    message = "Associated account not found!";
                }
            }
            catch (Exception e)
            {
                //most likely invalid Guid
                message = e.Message;
            }

            ViewBag.Message = message;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VerifyResetPassword(Maker model)
        {
            bool statusOK = false;
            string message = null;

            var maker = db.Makers.Where(a => a.ResetPasswordCode == model.ResetPasswordCode).FirstOrDefault();
            if (maker != null)
            {
                maker.Password = Encrypt.Hash(model.Password);
                maker.ResetPasswordCode = null;
                db.Configuration.ValidateOnSaveEnabled = false; // skip model validation
                db.SaveChanges();
                message = "New password updated successfully";
                statusOK = true;
            }
            else
            {
                message = "Invalid reset code";
            }

            ViewBag.Message = message;
            ViewBag.StatusOK = statusOK;
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [NonAction]
        public bool IsEmailExist(string email)
        {
            var maker = db.Makers.Where(a => a.Email == email).FirstOrDefault();
            return maker != null;
        }

        [NonAction]
        public void SendVerificationEmail(string email, string activationCode)
        {
            var verifyUrl = "/Makers/VerifyAccount/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            //string adminEmail = Properties.Settings.Default.adminEmail;
            //string adminName = Properties.Settings.Default.adminName;
            //string adminPassword = Properties.Settings.Default.adminPassword;

            string adminEmail = ConfigurationManager.AppSettings["adminEmail"];
            string adminName = ConfigurationManager.AppSettings["adminName"];
            string adminPassword = ConfigurationManager.AppSettings["adminPassword"];

            var fromEmail = new MailAddress(adminEmail, adminName);
            var toEmail = new MailAddress(email);
            var fromEmailPassword = adminPassword;

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(adminEmail, adminName);
            mail.To.Add(email);
            mail.Subject = "You Quizmint account has been created!";
            mail.Body = "<br />Please click link below to verify your account" +
                "<br /><br /><a href='" + link + "'>" + link + "</a>";
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("smtp.1and1.com");
            smtp.Credentials = new NetworkCredential(adminEmail, adminPassword);
            smtp.Send(mail);
        }

        [NonAction]
        public void SendForgotPasswordEmail(string email, string passwordResetCode)
        {
            var verifyUrl = "/Makers/VerifyResetPassword/" + passwordResetCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            //string adminEmail = Properties.Settings.Default.adminEmail;
            //string adminName = Properties.Settings.Default.adminName;
            //string adminPassword = Properties.Settings.Default.adminPassword;

            string adminEmail = ConfigurationManager.AppSettings["adminEmail"];
            string adminName = ConfigurationManager.AppSettings["adminName"];
            string adminPassword = ConfigurationManager.AppSettings["adminPassword"];

            var fromEmail = new MailAddress(adminEmail, adminName);
            var toEmail = new MailAddress(email);
            var fromEmailPassword = adminPassword;

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(adminEmail, adminName);
            mail.To.Add(email);
            mail.Subject = "Request for password reset";
            mail.Body = "<br />Please click link below to reset your password" +
                "<br /><br /><a href='" + link + "'>" + link + "</a>";
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("smtp.1and1.com");
            smtp.Credentials = new NetworkCredential(adminEmail, adminPassword);
            smtp.Send(mail);
        }

        // GET: Makers/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            Session["ProjectId"] = null;
            if (id == null || id != Int32.Parse(Session["MakerId"].ToString()))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Maker maker = db.Makers.Find(Session["MakerId"]);
            if (maker == null)
            {
                return HttpNotFound();
            }
            return View(maker);
        }

        // GET: Makers/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null || id != Int32.Parse(Session["MakerId"].ToString()))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Maker maker = db.Makers.Find(id);
            if (maker == null)
            {
                return HttpNotFound();
            }
            return View(maker);
        }

        // POST: Makers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Maker maker)
        {
            string message = null;
            Maker this_maker = db.Makers.Find(maker.Id);
            if (this_maker != null)
            {
                this_maker.FirstName = maker.FirstName;
                this_maker.LastName = maker.LastName;
                db.Configuration.ValidateOnSaveEnabled = false;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = maker.Id });
            }
            else
            {
                message = "Maker not found";
            }

            ViewBag.Message = message;
            return View();
        }   
    }
}
