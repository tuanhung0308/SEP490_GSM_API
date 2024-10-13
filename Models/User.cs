using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Alpha_API.Models
{
	public class User
	{
		public string      UserId { get; set; }
		public string      Name     { get; set; }
		public string      Email     { get; set; }	       
		public string      Password  { get; set; }    
		public string      Gender     { get; set; }
		public DateTime   Dob { get; set; }  
		public string     Address      { get; set; }
		public string     Phone        { get; set; }
		public string     RoleId             { get; set; }
		public string     UserAvatar         { get; set; }

	}				   
}
