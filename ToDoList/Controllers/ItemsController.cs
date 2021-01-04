using Microsoft.AspNetCore.Mvc;
using ToDoList.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace ToDoList.Controllers
{
  public class ItemsController : Controller
  {
    private readonly ToDoListContext _db;

    public ItemsController(ToDoListContext db)
    {
      _db = db;
    }

    public ActionResult Index()
    {
      return View(_db.Items.ToList());
    }
    public ActionResult Details(int id)
    {
      var thisItem = _db.Items
          .Include(item => item.Categories) //item refers to individual items in the Item database table 
          .ThenInclude(join => join.Category) //join is the linq thing. looks for connection between Item and Category tables. 
          .FirstOrDefault(item => item.ItemId == id); //specifies which particular item to run these commands on / filter list by 
      return View(thisItem);
    }

    public ActionResult Create()
    {
        ViewBag.CategoryId = new SelectList(_db.Categories, "CategoryId", "Name");
        return View();
    }

    [HttpPost]
    public ActionResult Create(Item item, int CategoryId)
    {
        _db.Items.Add(item);
        if (CategoryId != 0)
        {
          _db.CategoryItem.Add(new CategoryItem() { CategoryId = CategoryId, ItemId = item.ItemId });
        }
        _db.SaveChanges();
        return RedirectToAction("Index");
    }

    public ActionResult Edit(int id)
    {
        var thisItem = _db.Items.FirstOrDefault(items => items.ItemId == id);
        ViewBag.CategoryId = new SelectList(_db.Categories, "CategoryId", "Name");
        return View(thisItem);
    }

    [HttpPost]
    public ActionResult Edit(Item item, int CategoryId)
    {
      if (CategoryId !=0)
    {
      _db.CategoryItem.Add(new CategoryItem() { CategoryId = CategoryId, ItemId = item.ItemId });
    }
        _db.Entry(item).State = EntityState.Modified;
        _db.SaveChanges();
        return RedirectToAction("Index");
    }

    public ActionResult AddCategory(int id)
    {
        var thisItem = _db.Items.FirstOrDefault(items => items.ItemId == id);
        ViewBag.CategoryId = new SelectList(_db.Categories, "CategoryId", "Name");
        return View(thisItem);
    }

    [HttpPost]
    public ActionResult AddCategory(Item item, int CategoryId)
    {
      if (CategoryId != 0)
      {
        _db.CategoryItem.Add(new CategoryItem() { CategoryId = CategoryId, ItemId = item.ItemId }); 
      }
      _db.SaveChanges(); 
      return RedirectToAction("Index"); 
    }

    public ActionResult Delete(int id)
    {
        var thisItem = _db.Items.FirstOrDefault(items => items.ItemId == id);
        return View(thisItem);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
        var thisItem = _db.Items.FirstOrDefault(items => items.ItemId == id);
        _db.Items.Remove(thisItem);
        _db.SaveChanges();
        return RedirectToAction("Index");
    }

    [HttpPost]
    public ActionResult DeleteCategory(int joinId)
    {
      var joinEntry = _db.CategoryItem.FirstOrDefault(entry => entry.CategoryItemId == joinId);
      _db.CategoryItem.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  }
}