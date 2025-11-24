using FluentValidation;
using MinimalAPI.Dto;

namespace MinimalAPI.Validation
{
    public class CouponCreateValidation : AbstractValidator<CouponDtoWithoutId>
    {
        public CouponCreateValidation()
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");
            RuleFor(c => c.Percent)
                .InclusiveBetween(0, 100).WithMessage("Percent must be between 0 and 100.");
            RuleFor(c => c.IsActive)
                .NotNull().WithMessage("IsActive is required.");
        }
    }
    public class CouponUpdateValidation : AbstractValidator<CouponDto>
    {
        public CouponUpdateValidation()
        {
            RuleFor(c => c.Id)
                .GreaterThan(0).WithMessage("Id must be greater than 0.");
        }
    }
}
