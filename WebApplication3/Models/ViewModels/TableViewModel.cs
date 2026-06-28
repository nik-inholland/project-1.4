namespace WebApplication3.Models.ViewModels
{
    public class MenuViewModel
    {
        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();

       
        public string SelectedCard { get; set; } = "All";          
        public int SelectedCourseType { get; set; } = 0;           

        
        public bool HasItems => MenuItems.Any();

        public MenuViewModel() { }

        public MenuViewModel(List<MenuItem> menuItems, string selectedCard = "All", int selectedCourse = 0)
        {
            MenuItems = menuItems;
            SelectedCard = selectedCard;
            SelectedCourseType = selectedCourse;
        }
    }
}