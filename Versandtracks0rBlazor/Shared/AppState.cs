using System;
using Microsoft.AspNetCore.Components.Web;

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
    }


}