using System;
using Microsoft.AspNetCore.Components.Web;
using Versandtracks0rBlazor.Models;

namespace Versandtracks0rBlazor.Shared
{

    public class AppState
    {
        public string SelectedColour { get; private set; }

        public event Action<MouseEventArgs> OnAddButtonClicked;

        public void invokeOnAddButtonClicked(MouseEventArgs e)
        {
            this.OnAddButtonClicked.Invoke(e);
        }

        public event Action<Toast> OnShowToast;

        public void showToast(Toast toast)
        {
            this.OnShowToast.Invoke(toast);
        }

        public void showToast(string message, ToastType type)
        {
            Toast toast = new Toast
            {
                Message = message,
                ToastType = type
            };

            this.showToast(toast);
        }
    }


}