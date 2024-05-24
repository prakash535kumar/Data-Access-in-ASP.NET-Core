// Import namespaces needed for the controller
using BasicEfCoreDemo.Data;
using BasicEfCoreDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// Define namespace and class for the controller
namespace BasicEfCoreDemo.Controllers
{
    // Define the base route for the controller and indicate it is an API controller
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        // Define a private field to hold the context of the database
        private readonly InvoiceDbContext _context;

        // Constructor for the controller, injects the database context
        public InvoicesController(InvoiceDbContext context)
        {
            _context = context;
        }

        // HTTP GET method to retrieve all invoices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetInvoices()
        {
            // Check if the invoice context is null, return NotFound if so
            if (_context.Invoices == null)
            {
                return NotFound();
            }
            // Retrieve all invoices asynchronously and return as ActionResult
            return await _context.Invoices.ToListAsync();
        }

        // HTTP GET method to retrieve a specific invoice by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Invoice>> GetInvoice(Guid id)
        {
            // Check if the invoice context is null, return NotFound if so
            if (_context.Invoices == null)
            {
                return NotFound();
            }
            // Find the invoice by ID asynchronously
            var invoice = await _context.Invoices.FindAsync(id);
            // If the invoice is not found, return NotFound
            if (invoice == null)
            {
                return NotFound();
            }
            // Return the found invoice
            return invoice;
        }

        // HTTP PUT method to update an existing invoice
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInvoice(Guid id, Invoice invoice)
        {
            // Check if the provided ID matches the invoice's ID, return BadRequest if not
            if (id != invoice.Id)
            {
                return BadRequest();
            }
            // Set the state of the provided invoice as Modified in the context
            _context.Entry(invoice).State = EntityState.Modified;
            try
            {
                // Save changes to the database asynchronously
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // If concurrency exception occurs, check if invoice exists and return NotFound if not
                if (!InvoiceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    // Otherwise, rethrow the exception
                    throw;
                }
            }
            // Return NoContent if successful
            return NoContent();
        }

        // HTTP POST method to create a new invoice
        [HttpPost]
        public async Task<ActionResult<Invoice>> PostInvoice(Invoice invoice)
        {
            // Check if the invoice context is null, return Problem with an error message if so
            if (_context.Invoices == null)
            {
                return Problem("Entity set 'InvoiceDbContext.Invoices' is null.");
            }
            // Add the provided invoice to the context and save changes asynchronously
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();
            // Return a CreatedAtAction result with the created invoice
            return CreatedAtAction("GetInvoice", new { id = invoice.Id }, invoice);
        }

        // HTTP DELETE method to delete an invoice by ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(Guid id)
        {
            // Check if the invoice context is null, return NotFound if so
            if (_context.Invoices == null)
            {
                return NotFound();
            }
            // Find the invoice by ID asynchronously
            var invoice = await _context.Invoices.FindAsync(id);
            // If the invoice is not found, return NotFound
            if (invoice == null)
            {
                return NotFound();
            }
            // Remove the found invoice from the context and save changes asynchronously
            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();
            // Return NoContent if successful
            return NoContent();
        }

        // Helper method to check if an invoice exists by ID
        private bool InvoiceExists(Guid id)
        {
            // Return true if an invoice with the provided ID exists in the context, false otherwise
            return (_context.Invoices?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
