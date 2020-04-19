using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Xamarin.Forms;

namespace prj3beer.Services
{
    public interface IToastHandler
    {
        void LongToast(string msg);

        void ShortToast(string msg);
    }
}