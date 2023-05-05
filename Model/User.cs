﻿namespace spl.Model
{
    public class User
    {
        public int? Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string? UserType { get; set; }
        public int? IdBahagian { get; set; }
        public int? IdCawangan { get; set; }
        public int? IdUnit { get; set; }
        public int? IdStesen { get; set; }

        public Bahagian? Bahagian { get; set; }
        public Cawangan? Cawangan { get; set; }
        public Unit? Unit { get; set; }
        public Stesen? Stesen { get; set; }
    }
}
