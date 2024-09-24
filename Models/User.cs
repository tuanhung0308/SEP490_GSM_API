using System;
using System.Collections.Generic;

namespace Alpha_API.Models
{
	public class User
	{
		public string UserId { get; set; }
		public string UserAvatar { get; set; }
		public string UserImage { get; set; }
		public DateTime UserFirstLogin { get; set; }
		public bool UserIsEnabled { get; set; }
		public string UserPhone { get; set; }
		public string UserName { get; set; }
		public string UserPassHashed { get; set; }
		public string UserEmail { get; set; }
		public int RoleId { get; set; }
		public string Address { get; set; }
	}
}
