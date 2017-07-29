using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using Esoteric.UI;
using Esoteric.BLL.Interfaces;

namespace Bailiwick.UI
{
    /// <summary>
    /// This class provides the generic implementation of the <see cref="IProgressWithExceptions"/> interface
    /// </summary>
    /// <remarks>
    /// This class also provides a thread-safe implmentation of <see cref="INotifyPropertyChanged"/>
    /// </remarks>
    public class ProgressViewModel : IViewModel, IProgressUI, INotifyPropertyChanged
    {
        #region Data Properties

        /// <summary>
        /// Progress view description
        /// </summary>
        public virtual string Title
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// State of the progress bar
        /// </summary>
        public virtual bool IsIndeterminate
        {
            get { return true; }
        }

        /// <summary>
        /// Indicates the view model is actively tracking progress
        /// </summary>
        public bool IsActive
        {
            get
            {
                return (ProgressMin != ProgressMax) && ProgressMax != 0;
            }
        }

        public Visibility IsVisible
        {
            get
            {
                return isVisible;
            }
            set
            {
                isVisible = value;
                OnPropertyChanged("IsVisible");
            }
        }
        Visibility isVisible = Visibility.Collapsed;

        /// <summary>
        /// Holds the visibility status of the error message
        /// </summary>
        public Visibility IsErrorVisible
        {
            get { return isErrorVisible; }
            private set
            {
                if (isErrorVisible != value)
                {
                    isErrorVisible = value;
                    OnPropertyChanged("IsErrorVisible");
                }
            }
        }
        Visibility isErrorVisible = Visibility.Collapsed;

        /// <summary>
        /// Holds the minimum value of the progress bar
        /// </summary>
        public int ProgressMin
        {
            get
            {
                return progressMin;
            }
            set
            {
                if (progressMin != value)
                {
                    progressMin = value;
                    OnPropertyChanged("ProgressMin");
                }
            }
        }
        int progressMin;

        /// <summary>
        /// Holds the maximum value of the progress bar
        /// </summary>
        public int ProgressMax
        {
            get
            {
                return progressMax;
            }
            set
            {
                if (progressMax != value)
                {
                    progressMax = value;
                    OnPropertyChanged("ProgressMax");
                }
            }
        }
        int progressMax;

        /// <summary>
        /// Holds the current position of the progress bar
        /// </summary>
        public int ProgressValue
        {
            get
            {
                return progressValue;
            }
            set
            {
                if (progressValue != value)
                {
                    progressValue = value;
                    OnPropertyChanged("ProgressValue");
                }
            }
        }
        int progressValue;

        /// <summary>
        /// Holds the message to display to the user
        /// </summary>
        public string Message
        {
            get
            {
                return message ?? (message = string.Empty);
            }
            set
            {
                if (message != value)
                {
                    message = value;
                    OnPropertyChanged("Message");
                }
            }
        }
        string message;

        /// <summary>
        /// shows or hides cancel button
        /// </summary>
        public bool ShowCancelButton
        {
            get
            {
                return showCancelButton;
            }
            set
            {
                if (showCancelButton != value)
                {
                    showCancelButton = value;
                    OnPropertyChanged("ShowCancelButton");
                }
            }
        }
        bool showCancelButton;

        /// <summary>
        /// shows or hides additional text
        /// </summary>
        public bool ShowAdditionalMessage
        {
            get
            {
                return showAdditionalMessage;
            }
            set
            {
                if (showAdditionalMessage != value)
                {
                    showAdditionalMessage = value;
                    OnPropertyChanged("ShowAdditionalMessage");
                }
            }
        }
        bool showAdditionalMessage;


        /// <summary>
        /// additional text that appears under the progress bar and cancel button
        /// </summary>
        public string AdditionalMessage
        {
            get
            {
                return additionalMessage;
            }
            set
            {
                if (additionalMessage != value)
                {
                    additionalMessage = value;
                    OnPropertyChanged("AdditionalMessage");
                }
            }
        }
        string additionalMessage;


        /// <summary>
        /// Holds the list of exceptions that have occurred
        /// </summary>
        public ObservableCollection<Exception> Exceptions
        {
            get
            {
                return exceptions ?? (exceptions = new ObservableCollection<Exception>());
            }
        }
        ObservableCollection<Exception> exceptions;
        #endregion

        #region IProgressUI Members
        /// <summary>
        /// Called at the beginning of the process. Reset the values and prepares for progress updates.
        /// </summary>
        virtual public void Beginning()
        {
            (CancelEvent as ManualResetEvent).Reset();
            ProgressValue = 0;
            //ProgressMin = 0; 
            //ProgressMax = 0;
            IsErrorVisible = Visibility.Collapsed;
            IsVisible = Visibility.Visible;

            // Make sure the collection is cleared in the UI thread
            Application.Current.Dispatcher.Invoke(new Action(Exceptions.Clear));
        }

        /// <summary>
        /// Called to display a message to the user
        /// </summary>
        /// <param name="caption">The message to display</param>
        virtual public void SetMessage(string caption)
        {
            Message = caption;
        }

        /// <summary>
        /// Called to set the minimum and maximum values of the progress bar
        /// </summary>
        /// <param name="min">The minimum value</param>
        /// <param name="max">The maximum value</param>
        virtual public void SetMinAndMax(int min, int max)
        {
            if (ProgressValue < min) ProgressValue = min;
            if (ProgressValue > max) ProgressValue = max;

            ProgressMin = min;
            ProgressMax = max;
        }

        /// <summary>
        /// Called to indicate a step has been completed
        /// </summary>
        virtual public void Increment()
        {
            ProgressValue += 1;
        }

        public void IncrementTo(int value)
        {
            ProgressValue = value;
        }

        /// <summary>
        /// Provides a way of signaling the process should be cancelled
        /// </summary>
        public WaitHandle CancelEvent
        {
            get
            {
                return cancelEvent ?? (cancelEvent = new ManualResetEvent(false));
            }
        }
        ManualResetEvent cancelEvent;

        /// <summary>
        /// Called at the end of the process. Finalizes any progress updates
        /// </summary>
        virtual public void Finished()
        {
            IsVisible = Visibility.Collapsed;

            ProgressValue = 0;
            ProgressMin = 0;
            ProgressMax = 0;
        }
        #endregion

        #region IProgressWithExceptions Members
        /// <summary>
        /// Called when an exception has occurred
        /// </summary>
        /// <param name="exception">The exception that occurred</param>
        public void AddException(Exception exception)
        {
            // Add the exception to the collection in the UI thread
            Application.Current.Dispatcher.Invoke(new Action<Exception>(Exceptions.Add), exception);

            IsErrorVisible = Visibility.Visible;
        }
        #endregion

        #region IViewModel Members
        /// <summary>
        /// Holds the user control or window associated with this view model
        /// </summary>
        public IView View
        {
            get;
            set;
        }
        #endregion

        #region INotifyPropertyChanged Members
        /// <summary>
        /// Raised whenever a property is changed in the view model
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Handles the logic of raising the PropertyChanged event
        /// </summary>
        /// <param name="info">The name of the property that was changed</param>
        /// <remarks>
        /// This call is forwarded to the UI thread
        /// </remarks>
        protected void OnPropertyChanged(string info)
        {
            Application.Current.Dispatcher.Invoke(new AsyncOnPropertyChanged(UISafeOnPropertyChanged), info);
        }

        delegate void AsyncOnPropertyChanged(string info);
        /// <summary>
        /// Raises the PropertyChanged event on the UI thread
        /// </summary>
        /// <param name="info">The name of the property that was changed</param>
        void UISafeOnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                // Make sure this happens on the UI thread
                handler(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion
    }
}
