namespace Nop.Plugin.Misc.BambooCardApi.DTO
{
    public class OrderDetailsDto
    {
        public int Id { get; set; }

        public decimal TotalAmount { get; set; }

        public DateOnly OrderDate { get; set; }
    }
}