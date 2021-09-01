using Autorentool_RMT.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Autorentool_RMT.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContactPage : ContentPage
    {

        private ContactViewModel contactViewModel;

        public ContactPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            contactViewModel = new ContactViewModel();
            BindingContext = contactViewModel;
        }

        #region OnSendButtonClicked
        /// <summary>
        /// Tries to send the email and displays a success prompt.
        /// If the sending process failed an error prompt will be displayed depending where the error happened.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnSendButtonClicked(object sender, EventArgs e)
        {
            try
            {
                bool response = await contactViewModel.OnSendMail();

                if (response)
                {
                    await DisplayAlert("Erfolg", "Ihre Nachricht wurde erfolgreich übermittelt", "Schließen");
                } else
                {
                    throw new Exception();
                }

            } catch(Exception exc)
            {
                if (exc.Message.Equals("Kein korrektes E-Mail-Format"))
                {
                    await DisplayAlert("Fehlerhafte E-Mail-Adresse", exc.Message, "Schließen");
                }
                else
                {
                    await DisplayAlert("Fehler beim Senden Ihrer Nachricht", "Das Übertragen Ihrere Nachricht ist fehlgeschlagen", "Schließen");
                }
            }
        }
        #endregion

    }
}