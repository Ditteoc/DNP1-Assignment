﻿using System.Text.Json;
using DTOs;

namespace BlazorApp.Services;

public class HttpPostService : IPostService
{
    private readonly HttpClient client;

    public HttpPostService(HttpClient client)
    {
        this.client = client;
    }

    // Opretter et nyt indlæg

    // Henter et specifikt indlæg
    public async Task<PostDTO> CreatePostAsync(CreatePostDTO request)
    {
        Console.WriteLine($"Sending request to create post: {JsonSerializer.Serialize(request)}");

        HttpResponseMessage response = await client.PostAsJsonAsync("posts", request);
        string responseString = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"API Response: {response.StatusCode} - {responseString}");

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(responseString);
        }

        return JsonSerializer.Deserialize<PostDTO>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    

    public async Task<PostDTO> GetPostAsync(int id)
    {
        HttpResponseMessage response = await client.GetAsync($"posts/{id}");
        string responseString = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(responseString);
        }
        
        return JsonSerializer.Deserialize<PostDTO>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    // Henter alle indlæg
    public async Task<IEnumerable<PostDTO>> GetPostsAsync()
    {
        try
        {
            HttpResponseMessage response = await client.GetAsync("posts");
            string responseString = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"API Response Status: {response.StatusCode}");
            Console.WriteLine($"API Response Content: {responseString}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"HTTP {response.StatusCode}: {responseString}");
            }

            if (string.IsNullOrWhiteSpace(responseString))
            {
                throw new Exception("API returned an empty response.");
            }

            var posts = JsonSerializer.Deserialize<IEnumerable<PostDTO>>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (posts == null)
            {
                throw new Exception("Failed to deserialize API response to PostDTO.");
            }

            return posts;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetPostsAsync: {ex.Message}");
            throw; // Rethrow the exception to propagate it
        }
    }


    // Opdaterer et eksisterende indlæg
    public async Task UpdatePostAsync(int id, UpdatePostDTO request)
    {
        HttpResponseMessage response = await client.PutAsJsonAsync($"posts/{id}", request);
        string responseString = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(responseString);
        }
    }

    // Sletter et specifikt indlæg
    public async Task DeletePostAsync(int id)
    {
        HttpResponseMessage response = await client.DeleteAsync($"posts/{id}");
        string responseString = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(responseString);
        }
    }

    public async Task AddPostAsync(CreatePostDTO post)
    {
        var response = await client.PostAsJsonAsync("posts", post);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to add post: {content}");
        }
    }
}