using AutoMapper;
using Domain;
using Domain.Dto;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using System;
using System.Linq;
using GemBox.Document;
using GemBox.Document.Tables;

namespace Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController : Controller
{
    private readonly IOrderService _orderService;
    private readonly IMapper _mapper;

    public OrderController(IOrderService orderService, IMapper mapper)
    {
        _orderService = orderService;
        _mapper = mapper;
    }

    [HttpGet("getAllOrders/")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Order>))]
    public IActionResult GetOrders()
    {
        var result = _mapper.Map<List<OrderDto>>(this._orderService.getAllOrders());
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(result);
    }

    [HttpPost("createInvoice/{orderId}")]
    public ActionResult CreateInvoice(int orderId)
    {
        var order = _mapper.Map<OrderDto>(_orderService.getOrderDetails(orderId));

        ComponentInfo.SetLicense("FREE-LIMITED-KEY");

        int numberOfItems = 10;

        // string absoluteFilePath = @"C:\Users\lenovo\Desktop\IS_Project\IS_Project_Backend\Web\Invoice.docx";
        // DocumentModel document = DocumentModel.Load(absoluteFilePath);

        DocumentModel document = DocumentModel.Load("Invoice.docx");

        Table[] tables = document.GetChildElements(true, ElementType.Table).Cast<Table>().ToArray();

        // First table contains invoice number and date.
        Table invoiceTable = tables[0];
        invoiceTable.Rows[0].Cells[1].Blocks.Add(new Paragraph(document, order.Id.ToString()));
        // invoiceTable.Rows[1].Cells[1].Blocks.Add(new Paragraph(document, order.User.FirstName));
        // invoiceTable.Rows[2].Cells[1].Blocks.Add(new Paragraph(document, order.User.LastName));
        // invoiceTable.Rows[3].Cells[1].Blocks.Add(new Paragraph(document, order.User.Username));
        // invoiceTable.Rows[4].Cells[1].Blocks.Add(new Paragraph(document, order.User.Email));

        // // Third table contains amount and prices, it only has one data row in the template document.
        // // So, we'll dynamically add cloned rows for the rest of our data items.
        // Table mainTable = tables[2];
        // for (int i = 1; i < numberOfItems; i++)
        //     mainTable.Rows.Insert(1, mainTable.Rows[1].Clone(true));
        //
        // int total = 0;
        // for (int rowIndex = 1; rowIndex <= numberOfItems; rowIndex++)
        // {
        //     DateTime date = DateTime.Today.AddDays(rowIndex - numberOfItems);
        //     int hours = rowIndex % 3 + 6;
        //     int unit = 35;
        //     int price = hours * unit;
        //     
        //     mainTable.Rows[rowIndex].Cells[0].Blocks.Add(new Paragraph(document, date.ToString("d MMM yyyy")));
        //     mainTable.Rows[rowIndex].Cells[1].Blocks.Add(new Paragraph(document, hours.ToString()));
        //     mainTable.Rows[rowIndex].Cells[2].Blocks.Add(new Paragraph(document, unit.ToString("0.00")));
        //     mainTable.Rows[rowIndex].Cells[3].Blocks.Add(new Paragraph(document, price.ToString("0.00")));
        //
        //     total += price;
        // }

        // Last cell in the last, total, row has some predefined formatting stored in an empty paragraph.
        // So, in this case instead of adding new paragraph we'll add our data into an existing paragraph.
        // mainTable.Rows.Last().Cells[3].Blocks.Cast<Paragraph>(0).Content.LoadText(total.ToString("0.00"));

        // Fourth table contains notes.

        string fileName = $"Order_{order.Id}.docx";
        
        document.Save(fileName);
        return Ok("Invoice saved");
    }
}