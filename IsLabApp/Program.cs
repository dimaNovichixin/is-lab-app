using IsLabApp.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Хранилище заметок (в памяти)
var notes = new List<Note>();
var nextId = 1;

// GET /health
app.MapGet("/health", () =>
{
    return Results.Ok(new
    {
        status = "ok",
        time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
    });
});

// GET /version
app.MapGet("/version", (IConfiguration config) =>
{
    var appName = config["App:Name"] ?? "IsLabApp";
    var appVersion = config["App:Version"] ?? "1.0.0";

    return Results.Ok(new
    {
        name = appName,
        version = appVersion
    });
});

// GET /api/notes - получить все заметки
app.MapGet("/api/notes", () =>
{
    return Results.Ok(notes);
});

// GET /api/notes/{id} - получить заметку по id
app.MapGet("/api/notes/{id}", (int id) =>
{
    var note = notes.FirstOrDefault(n => n.Id == id);
    if (note == null)
        return Results.NotFound(new { message = "Заметка не найдена" });

    return Results.Ok(note);
});

// POST /api/notes - создать заметку
app.MapPost("/api/notes", (NoteInput input) =>
{
    // Валидация
    if (string.IsNullOrWhiteSpace(input.Title))
        return Results.BadRequest(new { message = "Title обязателен" });

    if (string.IsNullOrWhiteSpace(input.Text))
        return Results.BadRequest(new { message = "Text обязателен" });

    var note = new Note
    {
        Id = nextId++,
        Title = input.Title,
        Text = input.Text,
        CreatedAt = DateTime.Now
    };

    notes.Add(note);
    return Results.Created($"/api/notes/{note.Id}", note);
});

// DELETE /api/notes/{id} - удалить заметку
app.MapDelete("/api/notes/{id}", (int id) =>
{
    var note = notes.FirstOrDefault(n => n.Id == id);
    if (note == null)
        return Results.NotFound(new { message = "Заметка не найдена" });

    notes.Remove(note);
    return Results.Ok(new { message = "Заметка удалена", id = id });
});

app.Run();

// DTO для создания заметки
public class NoteInput
{
    public string Title { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}