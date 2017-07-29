using Esoteric.UI;
using System.Windows.Controls;

namespace Bailiwick.UI
{
    /// <summary>
    /// Interaction logic for WordFrequencyOptions.xaml
    /// </summary>
    public partial class WordFrequencyOptions : ToolBar, IView
    {
        public WordFrequencyOptions()
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
