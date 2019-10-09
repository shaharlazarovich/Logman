using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Logman.Common.DomainObjects;

namespace Logman.Web.Models.Apps
{
    public class AppUsersViewModel
    {
        private readonly List<User> _allUsers;

        public AppUsersViewModel()
        {
            _allUsers = new List<User>();
        }

        public List<User> AppUsers
        {
            get { return _allUsers; }
        }

        [Required]
        public long AppId { get; set; }

        [Required]
        public string AppName { get; set; }
    }
}