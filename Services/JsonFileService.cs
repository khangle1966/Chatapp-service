using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using dotnet_chat.Dtos;

using Microsoft.AspNetCore.Hosting;

namespace dotnet_chat.Services
{
    public class JsonFileService
    {
        private readonly string _filePath;
        private readonly string _messageFilePath;

        public JsonFileService(IWebHostEnvironment webHostEnvironment)
        {
            _filePath = Path.Combine(webHostEnvironment.ContentRootPath, "Data", "users.json");
            _messageFilePath = Path.Combine(webHostEnvironment.ContentRootPath, "Data", "messages.json");
        }

        public List<UserDTO> GetUsers()
        {
            using (var jsonFileReader = File.OpenText(_filePath))
            {
                return JsonSerializer.Deserialize<List<UserDTO>>(jsonFileReader.ReadToEnd(),
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
            }
        }

        public void SaveUsers(List<UserDTO> users)
        {
            using (var outputStream = File.OpenWrite(_filePath))
            {
                JsonSerializer.Serialize<IEnumerable<UserDTO>>(
                    new Utf8JsonWriter(outputStream, new JsonWriterOptions
                    {
                        SkipValidation = true,
                        Indented = true
                    }),
                    users
                );
            }
        }

        public void AddUser(UserDTO user)
        {
            var users = GetUsers();
            users.Add(user);
            SaveUsers(users);
        }

        public UserDTO GetUser(string username)
        {
            return GetUsers().FirstOrDefault(user => user.Username == username);
        }

        public void AddMessage(MessageDTO message)
        {
            var messages = GetMessages();
            messages.Add(message);
            SaveMessages(messages);
        }

        public List<MessageDTO> GetMessages()
        {
            if (!File.Exists(_messageFilePath))
            {
                return new List<MessageDTO>();
            }

            using (var jsonFileReader = File.OpenText(_messageFilePath))
            {
                return JsonSerializer.Deserialize<List<MessageDTO>>(jsonFileReader.ReadToEnd(),
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
            }
        }

        public void SaveMessages(List<MessageDTO> messages)
        {
            using (var outputStream = File.OpenWrite(_messageFilePath))
            {
                JsonSerializer.Serialize<IEnumerable<MessageDTO>>(
                    new Utf8JsonWriter(outputStream, new JsonWriterOptions
                    {
                        SkipValidation = true,
                        Indented = true
                    }),
                    messages
                );
            }
        }
    }
}
