using System.Windows.Controls;
using Esoteric.UI;

namespace Bailiwick.UI
{
    /// <summary>
    /// Interaction logic for DecoratedSentenceView.xaml
    /// </summary>
    public partial class DecoratedSentenceView : UserControl, IView
    {
        public DecoratedSentenceView()
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
