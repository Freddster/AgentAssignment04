using System.Collections.ObjectModel;

namespace AgentAssignment
{
    public class Specialities : ObservableCollection<string>
    {
        public Specialities()
        {
            Add("Assassination");
            Add("Bombs");
            Add("Martinis");
            Add("Low profile");
            Add("Spy");
        }
    }
}