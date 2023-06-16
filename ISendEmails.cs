using Birthminder.Data;
using Microsoft.AspNetCore.Mvc;

namespace Birthminder
{
    public interface ISendEmails 
    {
        void checkUsersDates();
        void sendMailForBirthDay(User user);
        void sendMailForWishlist(User user, int count);
    }
}