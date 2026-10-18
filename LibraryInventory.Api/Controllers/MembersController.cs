using FluentValidation;
using LibraryInventory.Api.Dtos.Members;
using LibraryInventory.Api.Extensions;
using LibraryInventory.Core.Commands;
using LibraryInventory.Core.Entities;
using LibraryInventory.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryInventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MembersController(IMemberService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(int page = 1, int pageSize = 20, CancellationToken ct = default)
    {
        var r = await service.GetAllAsync(page, pageSize, ct);
        return Ok(new { items = r.Items.Select(ToDto), r.Page, r.PageSize, r.TotalCount });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var r = await service.GetByIdAsync(id, ct);
        return r.IsSuccess ? Ok(ToDto(r.Data!)) : this.ToFailure(r.Error, r.Message);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateMemberDto dto, IValidator<CreateMemberDto> v, CancellationToken ct)
    {
        var vr = await v.ValidateAsync(dto, ct);
        if (!vr.IsValid) return this.ToValidationProblem(vr);
        var r = await service.CreateAsync(new CreateMemberCommand(dto.MemberName, dto.Email), ct);
        return r.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = r.Data!.MemberId }, ToDto(r.Data))
            : this.ToFailure(r.Error, r.Message);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateMemberDto dto, IValidator<UpdateMemberDto> v,
        CancellationToken ct)
    {
        var vr = await v.ValidateAsync(dto, ct);
        if (!vr.IsValid) return this.ToValidationProblem(vr);
        var r = await service.UpdateAsync(id, new UpdateMemberCommand(dto.MemberName, dto.Email), ct);
        return r.IsSuccess ? Ok(ToDto(r.Data!)) : this.ToFailure(r.Error, r.Message);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var r = await service.DeleteAsync(id, ct);
        return r.IsSuccess ? NoContent() : this.ToFailure(r.Error, r.Message);
    }

    private static MemberResponseDto ToDto(Member m)
    {
        return new MemberResponseDto(m.MemberId, m.MemberName, m.Email);
    }
}