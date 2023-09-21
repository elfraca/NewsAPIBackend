using System.Collections;

namespace Domain.Data
{
    public class User
    {
        public int UserId { get; set; }
        private static DateTimeOffset createdFullDate { get; set; }
        public long CreatedUnixDate { get; } = createdFullDate.ToUnixTimeSeconds();
        public int Karma { get; set; }
        public string About { get; set; } = null!;
        public ArrayList Submitted { get; set; } = new ArrayList();
    }
}
