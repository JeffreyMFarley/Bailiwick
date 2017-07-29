using Esoteric.UI;
using System.Windows.Controls;

namespace Bailiwick.UI
{
    /// <summary>
    /// Interaction logic for SentenceTypeFrequencyView.xaml
    /// </summary>
    public partial class SentenceTypeFrequencyView : UserControl, IView
    {
        public SentenceTypeFrequencyView()
        {
            InitializeComponent();
        }

        #region IView Members

        /// <summary>
        /// Provides the data context for this view
        /// </summary>
        public IViewModel Model
        {
            get
            {
                return this.DataContext as IViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }

        #endregion
	
    }
}
