﻿using Org.BouncyCastle.Utilities;
using RYT.Models.Entities;
using System.Text.RegularExpressions;

namespace RYT.Data
{
    public static class SeedData
	{
		public static IList<string> Roles { get; set; } = new List<string> { "admin", "student", "teacher" };
		public static IList<AppUser> Users { get; set; } = new List<AppUser> {
			new AppUser
            {
				Id = "148d2f6f-af7a-423a-baa2-f01d434d9b3a",
				Email = "tester1@example.com",
				UserName = "tester1@example.com",
				FirstName = "Tester",
				LastName = "One",
                EmailConfirmed = true
            },
            new AppUser
            {
                Id = "1db045d1-3844-48a9-856f-b70ab674eafc",
                Email = "tester2@example.com",
                UserName = "tester2@example.com",
                FirstName = "Tester",
                LastName = "Two",
                EmailConfirmed = true
            },
            new AppUser
            {
                Email = "tester3@example.com",
                UserName = "tester3@example.com",
                FirstName = "Tester",
                LastName = "Three",
                Id = "0ae8ae01-1c9b-4a23-a995-b3f7197b29a3",
                EmailConfirmed = true
            },
            new AppUser
            {
                Email = "tester4@example.com",
                UserName = "tester4@example.com",
                FirstName = "Tester",
                LastName = "Four",
                Id = "1ebe6087-5788-4281-8401-ee0e4ce5031d",
                EmailConfirmed = true
            },
            new AppUser
            {
                Email = "tester5@example.com",
                UserName = "tester5@example.com",
                FirstName = "Tester",
                LastName = "Five",
                Id = "902258f9-b3c2-42d0-b420-59d383a269dc",
                EmailConfirmed = true
            },
            new AppUser
            {
                Email = "tester6@example.com",
                UserName = "tester6@example.com",
                FirstName = "Tester",
                LastName = "Six",
                Id = "92d57368-0133-4fa6-b85c-2c5dd03cd802",
                EmailConfirmed = true
            }
        };

            public static IList<string> Schools { get; set; } = new List<string>
            {
                "Mainland Senior High School",
                "Wesley Girls Junior Secondary School",
                "Ebute-Metta High School",
                "Marywood Senior Grammar School",
                "Akoka Junior High School",
                "Herbert Macaulay Junior Secondary School",
                "Queens College",
                "St. Gregory's College",
                "Government College, Ikorodu",
                "Federal Government College, Lagos",
                "St. Finbarr's College",
                "Holy Child College",
                "CMS Grammar School",
                "Girls' Secondary Grammar School, Ikorodu",
                "Kings College",
                "Igbobi College",
                "Federal Science and Technical College, Yaba",
                "Ansar-Ud-Deen College, Isolo",
                "Lagos State Model College, Kankon",
                "Lagos State Senior Model College, Badore",
                "Yaba College of Technology Secondary School",
                "Oriwu Model College, Ikorodu",
                "Dowen College",
                "Lagos Anglican Girls Grammar School",
                "Caleb International College",
                "St. Louis College",
                "British International School",
                "Atlantic Hall",
                "Ronik Comprehensive School",
                "Holly Hall Academy",
                "Halifield Schools",
                "Chrisland College",
                "Topfaith International Secondary School",
                "Greenwood House School",
                "Doregos Private Academy",
                "Avi-Cenna International School",
                "Anwar ul-Islam Girls High School",
                "Atlantic Hall",
                "Lagoon Secondary School, Lekki",
                "Lagos Preparatory School",
                "Lagos State Junior Model College Badore",
                "Lagos State Junior Model College Kankon",
                "Lagos State Model College Badore",
                "Lagos State Model College, Igbonla",
                "Lagos State Model College Kankon",
                "Lagos State Model Junior College Meiran",
                "Lebanese Community School",
                "Lekki British School",
                "Logic Group of Schools",
                "Lycée Français Louis Pasteur de Lagos"

            };


        public static IList<string> Subjects { get; set; } = new List<string>
        {
            "Mathematics",
            "English",
            "Biology",
            "Geography",
            "Physics",
            "Technical Drawing",
            "Fine and Applied Art",
            "ECommerce"
        };

        public static IList<string> SchoolTypes { get; set; } = new List<string>
        {
            "Senior High School",
            "Senior Grammar School",
            "Junior Grammar School",
            "Secondary School"
        };
    }
}
