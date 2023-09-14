using AutoMapper;
using Domain;
using Domain.Dto;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using System;
using System.Linq;
using System.Text;
using ClosedXML.Excel;
using GemBox.Document;
using GemBox.Document.Tables;
using Microsoft.AspNetCore.Authorization;
using SaveOptions = GemBox.Document.SaveOptions;

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
    [Authorize("AdminPolicy")]
    public IActionResult GetOrders()
    {
        var result = _mapper.Map<List<OrderDto>>(this._orderService.getAllOrders());
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(result);
    }

    [HttpPost("createInvoice/{orderId}")]
    [Authorize("AdminPolicy")]
    public ActionResult CreateInvoice(int orderId)
    {
        var order = _mapper.Map<OrderDto>(_orderService.getOrderDetails(orderId));

        ComponentInfo.SetLicense("FREE-LIMITED-KEY");

        int numberOfItems = 10;

        DocumentModel document = DocumentModel.Load("Invoice.docx");

        Table[] tables = document.GetChildElements(true, ElementType.Table).Cast<Table>().ToArray();
        
        Table invoiceTable = tables[0];
        invoiceTable.Rows[0].Cells[1].Blocks.Add(new Paragraph(document, order.Id.ToString()));
        invoiceTable.Rows[1].Cells[1].Blocks.Add(new Paragraph(document, order.User.Username));
        invoiceTable.Rows[2].Cells[1].Blocks.Add(new Paragraph(document, order.User.Email));

        Table secondTable = tables[1];
        
        StringBuilder sb = new StringBuilder();

        var total = 0.0;

        foreach (var product in order.ProductInOrders)
        {
            total += product.Price * product.Quantity;
            sb.AppendLine(product.Name + " with quantity of: " + product.Quantity + " and price of : $" +
                          product.Price);
        }
        secondTable.Rows[0].Cells[1].Blocks.Add(new Paragraph(document, sb.ToString()));

        secondTable.Rows.Last().Cells[3].Blocks.Cast<Paragraph>(0).Content.LoadText(total.ToString("0.00"));

        byte[] documentBytes;
        using (MemoryStream stream = new MemoryStream())
        {
            document.Save(stream, SaveOptions.DocxDefault);
            documentBytes = stream.ToArray();
        }
        
        Response.Headers["Content-Disposition"] = $"attachment; filename=Order_{order.Id}_{order.User.Username}.docx";
        Response.Headers["Content-Type"] = "application/octet-stream";
        
        return File(documentBytes, "application/octet-stream");
    }
    
    [HttpGet]
    [Route("exportAllOrders/")]
    [Authorize("AdminPolicy")]
    public FileContentResult ExportAllOrders()
    {
        var result = _orderService.getAllOrders();

        string fileName = "Orders.xlsx";
        string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        using (var workBook = new XLWorkbook())
        {
            IXLWorksheet worksheet = workBook.Worksheets.Add("All Orders");

            // Define column headers
            worksheet.Cell(1, 1).Value = "Order Id";
            worksheet.Cell(1, 2).Value = "Customer Name";
            worksheet.Cell(1, 3).Value = "Customer Last Name";
            worksheet.Cell(1, 4).Value = "Customer Username";
            worksheet.Cell(1, 5).Value = "Customer Email";

            int row = 2; // Start from the second row
            foreach (var item in result)
            {
                worksheet.Cell(row, 1).Value = item.Id.ToString();
                worksheet.Cell(row, 2).Value = item.User.FirstName;
                worksheet.Cell(row, 3).Value = item.User.LastName;
                worksheet.Cell(row, 4).Value = item.User.Username;
                worksheet.Cell(row, 5).Value = item.User.Email;

                int column = 6; // Start from the fifth column
                int counter = 1;
                foreach (var productInOrder in item.ProductInOrders)
                {
                    
                    worksheet.Cell(1, column).Value = "Product-" + counter;
                    worksheet.Cell(row, column).Value = productInOrder.Product.Name;
                    column++;
                    counter++;
                }

                row++;
            }

            using (var stream = new MemoryStream())
            {
                workBook.SaveAs(stream);

                var content = stream.ToArray();

                // Set response headers
                Response.Headers.Add("Content-Disposition", $"attachment; filename={fileName}");
                Response.Headers["Content-Type"] = contentType; 

                return File(content, contentType, fileName);
            }
        }
    }

}