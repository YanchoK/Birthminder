using Birthminder.Controllers;
using Birthminder.Data;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Birthminder
{

    public class SendEmails : Controller, ISendEmails
    {


        private readonly birthminderContext _context;

          public SendEmails(birthminderContext context)
          {
              _context = context;
          }

          public void checkUsersDates()
          {
              User[] users = _context.Users.ToArray();


              foreach (User user in users)
              {
                  //check if is in 2 weeks period
                  DateTime currentBirthdate = new DateTime(DateTime.Now.Year, user.BirthDate.Month, user.BirthDate.Day);
                  int dayDiff = (int)(currentBirthdate - DateTime.Today).TotalDays;
                  if (dayDiff == 14)
                  {
                      sendMailForBirthDay(user);
                  }

                  int countWishes = _context.Wishes.Count(wish => wish.UserId == user.Id && wish.IsDeleted==false);
                  if (countWishes < 3 && dayDiff == 21)
                  {
                      sendMailForWishlist(user, countWishes);
                  }

              }
          }


          public void sendMailForBirthDay(User user)
          {

              Team currentTeam = _context.Teams.Find(user.TeamId);
              User[] colleagues = _context.Users.Where(colleague => colleague.TeamId == currentTeam.Id && colleague.Id != user.Id).ToArray();

              var mailMessage = new MimeMessage();
              mailMessage.From.Add(new MailboxAddress("Birthminder Bulgaria", "birthminder.nemetschek@gmail.com"));

              try
              {
                  

                  foreach (User colleague in colleagues)
                  {
                        mailMessage.To.Add(new MailboxAddress(colleague.FullName, colleague.Email));
                    
                  
                        mailMessage.Subject = user.FullName + " Birthday!";

                    string myHostUrl = "https://school_2021/404/wishes/wishlist/";

                    mailMessage.Body = new TextPart("plain")
                      {

                          Text = "Hello " + colleague.FullName + ",\n"+
                          "your colleague from team " + currentTeam.Name + " has BIRTHDAY in 2 weeks!\n\n"+
                          "Go check desire wishes:" + myHostUrl + user.Id

                      };

                      using (var smtpClient = new SmtpClient())
                      {
                          smtpClient.Connect("smtp.gmail.com", 587, false);
                          smtpClient.Authenticate("birthminder.nemetschek@gmail.com", "Y2SNFy5Ljp7Env3");
                          smtpClient.Send(mailMessage);
                          smtpClient.Disconnect(true);
                      }

                  }
              }
            catch (Exception ex)
            {
                //throw (ex);
              }
          }

          public void sendMailForWishlist(User user, int countWishes)
          {


              var mailMessage = new MimeMessage();
              mailMessage.From.Add(new MailboxAddress("Birthminder Bulgaria", "birthminder.nemetschek@gmail.com"));

              try
              {
                  mailMessage.To.Add(new MailboxAddress(user.FullName, user.Email));
                  mailMessage.Subject = "Whishlist Reminder";


                  mailMessage.Body = new TextPart("plain")
                  {

                      Text = "Hello " + user.FullName + ",\n" + 
                      "you have " + countWishes + " wishes. \n"+
                      "Please add " + (3- countWishes) + " more!"
                  };

                  using (var smtpClient = new SmtpClient())
                  {
                      smtpClient.Connect("smtp.gmail.com", 587, false);
                      smtpClient.Authenticate("birthminder.nemetschek@gmail.com", "Y2SNFy5Ljp7Env3");
                      smtpClient.Send(mailMessage);
                      smtpClient.Disconnect(true);
                  }
              }
            catch (Exception ex)
            {
              //  throw (ex);
            }
        } 
        
    }

        
    
}
