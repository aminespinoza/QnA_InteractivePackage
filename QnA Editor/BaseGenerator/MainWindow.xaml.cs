using BaseGenerator.Helpers;
using System.Windows;
using System.Windows.Controls;

namespace BaseGenerator
{
    public partial class MainWindow : Window
    {
        private int carouselCounter = 5;
        private string carouselGeneratedAnswer = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnRegisterData_Click(object sender, RoutedEventArgs e)
        {
            tabEditor.Visibility = Visibility.Visible;
            QnAEditor.AssignCredentials(txtKnowledgeBaseName.Text, txtKnowledgeBaseId.Text, txtAuthorizationKey.Text);
        }

        private void CmbCardSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            string selectedText = (combo.SelectedItem as ComboBoxItem).Content.ToString();
            txtSelectedCard.Text = selectedText;

            gridVideoCard.Visibility = Visibility.Collapsed;
            gridHeroCard.Visibility = Visibility.Collapsed;
            gridPlainText.Visibility = Visibility.Collapsed;
            gridAudioCard.Visibility = Visibility.Collapsed;
            gridCarouselCard.Visibility = Visibility.Collapsed;

            switch (selectedText)
            {
                case "Video card":
                    gridVideoCard.Visibility = Visibility.Visible;
                    break;
                case "Plain text":
                    gridPlainText.Visibility = Visibility.Visible;
                    break;
                case "Hero card":
                    gridHeroCard.Visibility = Visibility.Visible;
                    break;
                case "Carousel card":
                    gridCarouselCard.Visibility = Visibility.Visible;
                    break;
                case "Audio card":
                    gridAudioCard.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void BtnAddPlainTextQuestion_Click(object sender, RoutedEventArgs e)
        {
            string generatedQuestion = txtPlainTextQuestion.Text;
            string generatedAnswer = string.Empty;
            string category = "TXT|";
            string plainAnswer = txtPlainText.Text;
            generatedAnswer = category + plainAnswer;

            QnAEditor.AddNewAnswer(generatedAnswer, generatedQuestion);

            txtPlainTextQuestion.Text = string.Empty;
            txtPlainTextQuestion.Text = string.Empty;

            MessageBox.Show("Your Plain Text question has been added, remember to publish your Knowledge base before start working with it");
        }

        private void BtnAddHeroCardQuestion_Click(object sender, RoutedEventArgs e)
        {
            string generatedQuestion = txtHeroCardQuestion.Text;
            string generatedAnswer = string.Empty;
            string category = "HRC|";
            string heroAnswer = txtHeroCardDescription.Text + "|" + txtHeroCardImage.Text;
            generatedAnswer = category + heroAnswer;

            QnAEditor.AddNewAnswer(generatedAnswer, generatedQuestion);

            txtHeroCardQuestion.Text = string.Empty;
            txtHeroCardDescription.Text = string.Empty;
            txtHeroCardImage.Text = string.Empty;

            MessageBox.Show("Your Hero Card question has been added, remember to publish your Knowledge base before start working with it");
        }

        private void BtnAddVideoCardQuestion_Click(object sender, RoutedEventArgs e)
        {
            string generatedQuestion = txtVideoCardQuestion.Text;
            string generatedAnswer = string.Empty;
            string category = "VDC|";
            string videoAnswer = txtVideoCardTitle.Text + "|" + txtVideoCardDescription.Text + "|" + txtVideoCardMedia.Text;
            generatedAnswer = category + videoAnswer;
            QnAEditor.AddNewAnswer(generatedAnswer, generatedQuestion);

            txtVideoCardQuestion.Text = string.Empty;
            txtVideoCardTitle.Text = string.Empty;
            txtVideoCardDescription.Text = string.Empty;
            txtVideoCardMedia.Text = string.Empty;

            MessageBox.Show("Your Video Card question has been added, remember to publish your Knowledge base before start working with it");
        }

        private void BtnAddAudioCardQuestion_Click(object sender, RoutedEventArgs e)
        {
            string generatedQuestion = txtAudioCardQuestion.Text;
            string generatedAnswer = string.Empty;
            string category = "ADC|";
            string audioAnswer = txtAudioCardTitle.Text + "|" + txtAudioCardDescription.Text + "|" + txtAudioCardMedia.Text;
            generatedAnswer = category + audioAnswer;
            QnAEditor.AddNewAnswer(generatedAnswer, generatedQuestion);

            txtAudioCardQuestion.Text = string.Empty;
            txtAudioCardTitle.Text = string.Empty;
            txtAudioCardDescription.Text = string.Empty;
            txtAudioCardMedia.Text = string.Empty;

            MessageBox.Show("Your Audio Card question has been added, remember to publish your Knowledge base before start working with it");
        }

        private void BtnAddNewCarouselElement_Click(object sender, RoutedEventArgs e)
        {
            txtCarouselCardQuestion.IsEnabled = false;

            if (carouselCounter == 0)
            {
                txtCarouselCardTitle.IsEnabled = false;
                txtCarouselCardDescription.IsEnabled = false;
                txtCarouselCardImage.IsEnabled = false;
                txtCarouselCardAction.IsEnabled = false;
                btnAddNewCarouselElement.IsEnabled = false;
                txtCarouselRemainingCounter.Text = "No more remaining items";
            }
            else
            {
                carouselGeneratedAnswer += txtCarouselCardTitle.Text + "|" + txtCarouselCardDescription.Text + "|" + txtCarouselCardImage.Text + "|";

                txtCarouselCardTitle.Text = string.Empty;
                txtCarouselCardDescription.Text = string.Empty;
                txtCarouselCardImage.Text = string.Empty;
                txtCarouselCardAction.Text = string.Empty;
                carouselCounter--;
                txtCarouselRemainingCounter.Text = $"{carouselCounter} remaining elements";         
            }           
        }

        private void BtnAddCarouselCardQuestion_Click(object sender, RoutedEventArgs e)
        {
            txtCarouselCardQuestion.IsEnabled = true;
            txtCarouselCardTitle.IsEnabled = true;
            txtCarouselCardDescription.IsEnabled = true;
            txtCarouselCardImage.IsEnabled = true;
            txtCarouselCardAction.IsEnabled = true;
            btnAddNewCarouselElement.IsEnabled = true;
            carouselCounter = 5;
            txtCarouselRemainingCounter.Text = $"{carouselCounter} remaining elements";


            string generatedQuestion = txtCarouselCardQuestion.Text;
            string generatedAnswer = string.Empty;
            string category = "CRC|";
            generatedAnswer = category + carouselGeneratedAnswer;

            txtCarouselCardQuestion.Text = string.Empty;
            txtCarouselCardTitle.Text = string.Empty;
            txtCarouselCardDescription.Text = string.Empty;
            txtCarouselCardImage.Text = string.Empty;
            txtCarouselCardAction.Text = string.Empty;

            MessageBox.Show("Your Carrousel Card question has been added, remember to publish your Knowledge base before start working with it");
        } 
    }
}
