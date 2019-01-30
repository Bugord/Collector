using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Collector.DAO.Entities
{
    public class Upload: BaseEntity
    {
        [StringLength(500)]
        [Required]
        public string Name { get; set; }
        [StringLength(500)]
        [Required]
        public string OriginalName { get; set; }
        [StringLength(500)]
        [Required]
        public string Extention { get; set; }
        [Required]
        public long Size { get; set; }
        [StringLength(500)]
        [Required]
        public string Path { get; set; }
        [Required]
        public UploadType Type { get; set; }
    }

    public enum UploadType
    {
        Avatar,
        Chat
    }
}
