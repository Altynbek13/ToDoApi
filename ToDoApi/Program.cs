using Microsoft.EntityFrameworkCore;
using ToDoApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDb>(db => db.UseInMemoryDatabase("TodoDb"));


var app = builder.Build();


app.MapGet("/todoitems", async (TodoDb db) => 
    await db.Todos.ToListAsync());

app.MapGet("/todoitems/{id}", async (int id, TodoDb db) =>
    await db.Todos.FindAsync(id));

app.MapPost("todoitems", async (TodoItem todo, TodoDb db) =>
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/todoitems/{todo.Id}",todo);
});

app.MapPut("/todoitems/{id}",async(int id,TodoItem newtodo,TodoDb db) =>
{
    var todo= await db.Todos.FindAsync(id);
    if (todo == null) return Results.NotFound();
    todo.Name= newtodo.Name;
    todo.IsComplete=newtodo.IsComplete;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/todoitems/{id}",async(int id,TodoDb db)=>{
    if (await db.Todos.FindAsync(id) is TodoItem todo)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
    return Results.NotFound();
});

app.Run();
