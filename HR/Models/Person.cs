using HR.Abstract;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Models
{
    [Table(name: "person", Schema = "public")]
    public class Person : HrBase
    {
        [Column("id_person")]
        [Key]
        public override int Id { get; set; }

        [Column("surname")]
        [JsonProperty(nameof(Surname))]
        [Required]
        [StringLength(50, ErrorMessage = "Длина поля не должна превышать 50 символов")]
        public string Surname { get; set; }

        [Column("firstname")]
        [JsonProperty(nameof(Firstname))]
        [Required]
        [StringLength(50, ErrorMessage = "Длина поля не должна превышать 50 символов")]
        public string Firstname { get; set; }

        [Column("patronymic")]
        [JsonProperty(nameof(Patronymic))]
        [StringLength(50, ErrorMessage = "Длина поля не должна превышать 50 символов")]
        public string? Patronymic { get; set; }

        [Column("gender")]
        [Required]
        [JsonProperty(nameof(Gender))]
        [StringLength(3, ErrorMessage = "Длина поля не должна превышать 3 символа")]
        public string Gender { get; set; }

        [Column("date_birth")]
        [JsonProperty(nameof(DateBirth))]
        public DateTime DateBirth { get; set; }

        [Column("phone_number")]
        [JsonProperty(nameof(PhoneNumber))]
        [StringLength(30, ErrorMessage = "Длина поля не должна превышать 30 символов")]
        public string? PhoneNumber { get; set; }

        [Column("email")]
        [JsonProperty(nameof(Email))]
        [StringLength(100, ErrorMessage = "Длина поля не должна превышать 100 символов")]
        public string? Email { get; set; }

        [Column("photo")]
        [JsonProperty(nameof(Photo))]
        public byte[]? Photo { get; set; }

        [NotMapped]
        [JsonProperty(nameof(FullName))]
        public string FullName
        {
            get
            {
                return $"{Surname} {Firstname} {Patronymic}";
            }
        }
        [NotMapped]
        public string DateBirthToString { get { return DateBirth.ToString("d"); } }

        [NotMapped]
        public int Age { get { return (int)(DateTime.Now - DateBirth).TotalDays / 365; } }

        [NotMapped]
        public int Experience { get; set; }


        [NotMapped]
        public string GetPhotoInBase64String
        {
            get
            {
                if (Photo != null)
                    return Convert.ToBase64String(Photo);
                else
                {
                    byte[]? people = null;
                    if (Gender == "Муж")
                    {
                        people = File.ReadAllBytes("wwwroot/files/man.png");

                    }
                    else if (Gender == "Жен")
                    {
                        people = File.ReadAllBytes("wwwroot/files/woman.png");
                    }
                    else 
                    {
                        return string.Empty;
                    }
                        return Convert.ToBase64String(people);
                }
            }
        }

        [JsonIgnore]
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }


}
