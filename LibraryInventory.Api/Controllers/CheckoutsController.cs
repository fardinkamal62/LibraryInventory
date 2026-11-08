using System.Security.Claims;
using FluentValidation;
using LibraryInventory.Api.Dtos.Checkouts;
using LibraryInventory.Api.Extensions;
using LibraryInventory.Core.Commands;
using LibraryInventory.Core.Common;
using LibraryInventory.Core.Entities;
using LibraryInventory.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryInventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CheckoutsController(ICheckoutService service, TimeProvider time) : ControllerBase
{
    [HttpPost(Name = "CreateCheckout")]
    public async Task<IActionResult> Create(CreateCheckoutDto dto, IValidator<CreateCheckoutDto> v,
        CancellationToken ct)
    {
        var vr = await v.ValidateAsync(dto, ct);
        if (!vr.IsValid) return this.ToValidationProblem(vr);

        // A member may only check out books for themselves; a librarian can check out for anyone.
        if (!User.IsInRole(Roles.Librarian)
            && User.FindFirstValue("memberId") != dto.MemberId.ToString())
            return Forbid();

        var r = await service.CheckoutAsync(new CreateCheckoutCommand(dto.BookId, dto.MemberId), ct);
        return r.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = r.Data!.CheckoutId }, ToDto(r.Data))
            : this.ToFailure(r.Error, r.Message);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var r = await service.GetByIdAsync(id, ct);
        if (r.IsFailure) return this.ToFailure(r.Error, r.Message);

        // Members can only read their own checkouts.
        if (!User.IsInRole(Roles.Librarian)
            && User.FindFirstValue("memberId") != r.Data!.MemberId.ToString())
            return Forbid();

        return Ok(ToDto(r.Data!));
    }

    [HttpPut("{id:int}/return")]
    [Authorize(Roles = Roles.Librarian)]
    public async Task<IActionResult> Return(int id, CancellationToken ct)
    {
        var r = await service.ReturnAsync(id, ct);
        return r.IsSuccess ? Ok(ToDto(r.Data!)) : this.ToFailure(r.Error, r.Message);
    }

    [HttpGet("overdue")]
    [Authorize(Roles = Roles.Librarian)]
    public async Task<IActionResult> Overdue(CancellationToken ct)
    {
        var now = time.GetUtcNow().UtcDateTime;
        return Ok((await service.GetOverdueAsync(ct)).Select(x => ToDto(x, now)));
    }

    private CheckoutResponseDto ToDto(Checkout c)
    {
        return ToDto(c, time.GetUtcNow().UtcDateTime);
    }

    private static CheckoutResponseDto ToDto(Checkout c, DateTime now)
    {
        return new CheckoutResponseDto(c.CheckoutId, c.BookId, c.MemberId, c.CheckedOutAtUtc, c.DueDateUtc,
            c.ReturnedAtUtc,
            c.IsOverdue(now), c.DaysOverdue(now));
    }
}
