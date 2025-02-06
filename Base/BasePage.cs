using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GestorIncidencias.Base
{
    public class BasePage : ContentPage
    {
        private BackButtonBehavior bb = new BackButtonBehavior
        {
            Command = new Command(async () =>
            {
                await Shell.Current.GoToAsync("..");
            })
        };

        public BasePage() : base()
        {
            Loaded += OnLoad;
        }

        private void OnLoad(object sender, EventArgs e)
        {
            setBackButtonBehavior();
        }

        protected virtual void setBackButtonBehavior()
        {
            Shell.SetBackButtonBehavior(this, bb);
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
        [CallerMemberName] string propertyName = "",
        Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
