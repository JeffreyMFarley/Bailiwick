using Esoteric.UI;
using System.Windows.Controls;

namespace Bailiwick.UI
{
    public partial class WordFrequencyView : UserControl, IView
    {
        public WordFrequencyView()
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
