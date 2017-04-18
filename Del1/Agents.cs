using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml.Serialization;
using MvvmFoundation.Wpf;

namespace AgentAssignment
{
    public class Agents : ObservableCollection<Agent>, INotifyPropertyChanged  // Just to reference it from xaml
    {
        public Agents()
        {
            if ((bool)(DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue))
            {
                Add(new Agent("007", "James Bond", "Killing", "Stop bad hombre"));
                Add(new Agent("001", "Nina", "Talking", "Talk people to death"));
            }
        }

        #region Commands

        #region Add Command

        ICommand _addCommand;
        public ICommand AddCommand
        {
            get { return _addCommand ?? (_addCommand = new RelayCommand(AddAgentExecute)); }
        }

        private void AddAgentExecute()
        {
            Add(new Agent());
            NotifyPropertyChanged("Count");
            CurrentIndex = Count - 1;
        }

        #endregion

        #region Remove Command

        private ICommand _removeCommand;
        public ICommand RemoveCommand
        {
            get { return _removeCommand ?? (_removeCommand = new RelayCommand(RemoveAgentExecute, RemoveAgent_CanExecute)); }
        }

        private void RemoveAgentExecute()
        {
            RemoveAt(CurrentIndex);
            NotifyPropertyChanged("Count");
        }

        //CanExecute gør at man ikke behøver at bygge fejlhåndtering ind i Execute funktionen
        private bool RemoveAgent_CanExecute()
        {
            if (Count > 0 && CurrentIndex >= 0)
                return true;
            else
                return false;
        }


        #endregion

        #region Previous Command

        private ICommand _prevAgentCommand;
        public ICommand PrevAgentCommand
        {
            get { return _prevAgentCommand ?? (_prevAgentCommand = new RelayCommand(PrevAgentExecute, PrevAgent_CanExecute)); }
            /* Alternativ get metode med lambda udtryk
             * get { return _prevAgentCommand ?? (_prevAgentCommand = new RelayCommand(
                () => --CurrentIndex,
                () => 0 < CurrentIndex )); }*/
        }
        private void PrevAgentExecute()
        {
            --CurrentIndex;
        }

        private bool PrevAgent_CanExecute()
        {
            if (0 < CurrentIndex)
                return true;
            else
                return false;
        }

        #endregion

        #region Next Command
        private ICommand _nextAgentCommand;

        public ICommand NextAgentCommand
        {
            get { return _nextAgentCommand ?? (_nextAgentCommand = new RelayCommand(NextAgentExecute, NextAgent_CanExecute)); }

            /* Alternativ get metode med lambda udtryk
             * get { return _nextAgentCommand ?? (_nextAgentCommand = new RelayCommand(
                () => ++CurrentIndex,
                () => CurrentIndex < (Count - 1))); }*/
        }

        private void NextAgentExecute()
        {
            ++CurrentIndex;
        }

        private bool NextAgent_CanExecute()
        {
            if (CurrentIndex < (Count - 1))
                return true;
            else
                return false;
        }

        #endregion

        private string _fileName;

        #region New File Command

        private ICommand _newFileCommand;

        public ICommand NewFileCommand
        {
            get { return _newFileCommand ?? (_newFileCommand = new RelayCommand(NewFileCommandExecute)); }
        }

        private void NewFileCommandExecute()
        {
            MessageBoxResult result =
                MessageBox.Show("Any unsaved data will be lost. Are you sure you want to initiate a new file?",
                    "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No /*<--- default result*/);
            if (result == MessageBoxResult.Yes)
            {
                Clear();
                _fileName = "";
            }
        }

        #endregion

        #region Open File Command

        private ICommand _openCommand;

        public ICommand OpenCommand
        {
            get { return _openCommand ?? (_openCommand = new RelayCommand<string>(OpenFileExecute)); }
        }

        private void OpenFileExecute(string fileName)
        {
            if (fileName == "")
            {
                MessageBox.Show("Enter a name for the file in the text box File Name", "Warning", MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
            }
            else
            {
                try
                {
                    _fileName = fileName;
                    Agents tempAgents = new Agents();
                    tempAgents.Clear();

                    //using (var stream = new FileStream(_fileName, FileMode.Open))
                    using (var stream = new StreamReader(_fileName))
                    {
                        XmlSerializer XML = new XmlSerializer(typeof(Agents));
                        tempAgents = (Agents)XML.Deserialize(stream);
                        stream.Close();
                    }

                    Clear();
                    foreach (var agent in tempAgents)
                    {
                        Add(agent);
                    }
                    NotifyPropertyChanged("Count");
                }
                catch (Exception e)
                {
                    MessageBox.Show("There is no file with the given name", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    //Console.WriteLine(e);
                    //throw;
                }
            }
        }

        #endregion

        #region Save As Command

        private ICommand _saveAsAgentCommand;

        public ICommand SaveAsAgentCommand
        {
            get
            {
                return _saveAsAgentCommand ?? (_saveAsAgentCommand = new RelayCommand<string>(SaveAsAgentExecute));
            }
        }

        private void SaveAsAgentExecute(string fileName)
        {
            if (fileName == "")
            {
                MessageBox.Show("Enter a name for the file in the text box File Name", "Warning", MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
            }
            else
            {
                _fileName = fileName;
                SaveAgentExecute();
            }
        }


        #endregion

        #region Save Agent Command

        private ICommand _saveAgentCommand;

        public ICommand SaveAgentCommand
        {
            get
            {
                return _saveAgentCommand ?? (_saveAgentCommand = new RelayCommand(SaveAgentExecute, SaveAgent_CanExecute));
            }
        }

        private void SaveAgentExecute()
        {
            try
            {
                //using (var stream = new FileStream(_fileName, FileMode.Create))   //Giver samme resultat
                using (var stream = new StreamWriter(_fileName))
                {
                    XmlSerializer XML = new XmlSerializer(typeof(Agents));
                    XML.Serialize(stream, this);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("There is no file with the given name to overwrite", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private bool SaveAgent_CanExecute()
        {
            return (_fileName != "") && (Count > 0);
        }

        #endregion

        #region Exit Command

        private ICommand _exitApplicationCommand;

        public ICommand ExitApplicationCommand
        {
            get
            {
                return _exitApplicationCommand ?? (_exitApplicationCommand = new RelayCommand(ExitApplicationExecute));
            }
        }

        private void ExitApplicationExecute()
        {
            Application.Current.Shutdown();
        }

        #endregion

        #endregion

        #region Properties

        private int currentIndex = -1;
        string filter;

        public int CurrentIndex
        {
            get { return currentIndex; }
            set
            {
                if (currentIndex != value)
                {
                    currentIndex = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public IReadOnlyCollection<string> FilterSpecialities
        {
            get
            {
                ObservableCollection<string> result = new ObservableCollection<string>();
                result.Add("All");
                foreach (var s in new Specialities())
                    result.Add(s);
                return result;
            }
        }

        int currentSpecialityIndex = 0;

        public int CurrentSpecialityIndex
        {
            get { return currentSpecialityIndex; }
            set
            {
                if (currentSpecialityIndex != value)
                {
                    ICollectionView cv = CollectionViewSource.GetDefaultView(this);
                    currentSpecialityIndex = value;
                    if (currentSpecialityIndex == 0)
                        cv.Filter = null; // Index 0 is no filter (show all)
                    else
                    {
                        filter = FilterSpecialities.ElementAt(currentSpecialityIndex);
                        cv.Filter = CollectionViewSource_Filter;
                    }
                    NotifyPropertyChanged();
                }
            }
        }

        private bool CollectionViewSource_Filter(object ag)
        {
            Agent agent = ag as Agent;
            return (agent.Speciality == filter);
        }

        #endregion

        #region INotifyPropertyChanged Implementation

        public new event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }

            //Alternativ body til hele funktionen
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        #endregion


    }
}