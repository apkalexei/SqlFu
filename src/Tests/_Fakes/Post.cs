﻿using System;
using CavemanTools.Model;
using CavemanTools.Model.ValueObjects;

namespace Tests._Fakes
{
    [Table("SomePost",IdentityColumn = "SomeId")]
    public class Post
    {
        public int SomeId { get; set; }
        public Guid Id { get; set; }
        public string Title { get; protected set; }
        public IdName Author { get; set; }
        public Email Email { get; set; }
        public DateTime CreatedOn { get; set; }

    }
}