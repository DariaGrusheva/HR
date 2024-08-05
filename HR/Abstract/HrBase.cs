using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Abstract
{
    public abstract class HrBase
    {
        public abstract int Id { get; set; }
        [Column("date_creation_record")]
        public DateTime DateCreationRecord { get; set; }

        [Column("modified_date_record")]
        public DateTime? ModifiedDateRecord { get; set; } = DateTime.Now;
    }
}
