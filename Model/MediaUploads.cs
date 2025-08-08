using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WhatsAppIntegration.Model
{
    public partial class MediaUploads
    {
        [Key]
        public string? Id {  get; set; }
        //public string Status { get; set; }
        //public string? FileName {  get; set; }
    }

    [Keyless]
    public partial class MediaUploadResponse
    {
        public List<MediaUploadData>? MediaUploadDataList {  get; set; }       
    }
    
    public partial class MediaUploadData
    {
        [Key]
        public int? Id {  get; set; }
        public string? Name {  get; set; }
        public string? Url {  get; set; }
        public string? MetaId {  get; set; }
        public string? MediaType {  get; set; }
        public bool? MetaSuccess {  get; set; }
        public bool? IsActive {  get; set; }
        
    }
}
