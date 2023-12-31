using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using Forumy.Models;

namespace Forumy.Controllers
{
    public class ForumController : Controller
    {
        private readonly IConfiguration _configuration;

        public ForumController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: ForumController
        public IActionResult ForumPage()
        {
            List<Forum> forums = GetForumsFromDatabase();
            return View(forums);
        }

        // Helper method to retrieve forums with comments from the database
        private List<Forum> GetForumsFromDatabase()
        {
            List<Forum> forums = new List<Forum>();

            using (MySqlConnection connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                string forumQuery = "SELECT f.Id AS ForumId, f.Title, f.Description, c.Id AS CommentId, c.UserName, c.Content " +
                                    "FROM forums f LEFT JOIN comments c ON f.Id = c.ForumId";

                using (MySqlCommand forumCmd = new MySqlCommand(forumQuery, connection))
                {
                    using (MySqlDataReader forumReader = forumCmd.ExecuteReader())
                    {
                        while (forumReader.Read())
                        {
                            int forumId = Convert.ToInt32(forumReader["ForumId"]);

                            Forum forum = forums.Find(f => f.Id == forumId);

                            if (forum == null)
                            {
                                forum = new Forum
                                {
                                    Id = forumId,
                                    Title = forumReader["Title"].ToString(),
                                    Description = forumReader["Description"].ToString(),
                                    Comments = new List<Comment>()
                                };

                                forums.Add(forum);
                            }

                            if (!forumReader.IsDBNull(forumReader.GetOrdinal("CommentId")))
                            {
                                Comment comment = new Comment
                                {
                                    Id = Convert.ToInt32(forumReader["CommentId"]),
                                    UserName = forumReader["UserName"].ToString(),
                                    Content = forumReader["Content"].ToString()
                                };

                                forum.Comments.Add(comment);
                            }
                        }
                    }
                }
            }

            return forums;
        }

        // GET: ForumController/CreateForum
       
        // POST: ForumController/CreateForum
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddForum(Forum forum)
        {
            if (ModelState.IsValid)
            {
                // Add logic to save the new forum to the database
                using (MySqlConnection connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();

                    string insertForumQuery = "INSERT INTO forums (Title, Description) VALUES (@Title, @Description)";
                    using (MySqlCommand insertForumCmd = new MySqlCommand(insertForumQuery, connection))
                    {
                        insertForumCmd.Parameters.AddWithValue("@Title", forum.Title);
                        insertForumCmd.Parameters.AddWithValue("@Description", forum.Description);

                        insertForumCmd.ExecuteNonQuery();
                    }
                }

                return RedirectToAction(nameof(ForumPage));
            }

            return RedirectToAction(nameof(ForumPage));
        }

        // GET: ForumController/Details/5
        public IActionResult Details(int id)
        {
            Forum forum = GetForumById(id);
            return View(forum);
        }

        // GET: ForumController/AddComment/5
       

        // POST: ForumController/AddComment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddComment(int forumId, Comment comment)
        {
            if (ModelState.IsValid)
            {
                // Add logic to save the new comment to the database
                using (MySqlConnection connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();

                    string insertCommentQuery = "INSERT INTO comments (ForumId, UserName, Content) VALUES (@ForumId, @UserName, @Content)";
                    using (MySqlCommand insertCommentCmd = new MySqlCommand(insertCommentQuery, connection))
                    {
                        insertCommentCmd.Parameters.AddWithValue("@ForumId", forumId);
                        insertCommentCmd.Parameters.AddWithValue("@UserName", comment.UserName);
                        insertCommentCmd.Parameters.AddWithValue("@Content", comment.Content);

                        insertCommentCmd.ExecuteNonQuery();
                    }
                }

                return RedirectToAction(nameof(ForumPage));
            }

            return RedirectToAction(nameof(ForumPage));
        }


        // GET: ForumController/EditForum/5
        public IActionResult EditForum(int id)
        {
            Forum forum = GetForumById(id);
            return View(forum);
        }

        // POST: ForumController/EditForum/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditForum(int id, Forum forum)
        {
            if (ModelState.IsValid)
            {
                // Add logic to update the forum in the database
                using (MySqlConnection connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();

                    string updateForumQuery = "UPDATE forums SET Title = @Title, Description = @Description WHERE Id = @ForumId";
                    using (MySqlCommand updateForumCmd = new MySqlCommand(updateForumQuery, connection))
                    {
                        updateForumCmd.Parameters.AddWithValue("@ForumId", id);
                        updateForumCmd.Parameters.AddWithValue("@Title", forum.Title);
                        updateForumCmd.Parameters.AddWithValue("@Description", forum.Description);

                        updateForumCmd.ExecuteNonQuery();
                    }
                }

                return RedirectToAction(nameof(ForumPage));
            }

            return View(forum);
        }

        // GET: ForumController/DeleteForum/5
        public IActionResult DeleteForum(int id)
        {
            Forum forum = GetForumById(id);
            return View(forum);
        }

        // POST: ForumController/DeleteForum/5
        [HttpPost, ActionName("DeleteForum")]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmDeleteForum(int id)
        {
            // Add logic to delete the forum and associated comments from the database
            using (MySqlConnection connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                // Delete comments associated with the forum
                string deleteCommentsQuery = "DELETE FROM comments WHERE ForumId = @ForumId";
                using (MySqlCommand deleteCommentsCmd = new MySqlCommand(deleteCommentsQuery, connection))
                {
                    deleteCommentsCmd.Parameters.AddWithValue("@ForumId", id);
                    deleteCommentsCmd.ExecuteNonQuery();
                }

                // Delete the forum
                string deleteForumQuery = "DELETE FROM forums WHERE Id = @ForumId";
                using (MySqlCommand deleteForumCmd = new MySqlCommand(deleteForumQuery, connection))
                {
                    deleteForumCmd.Parameters.AddWithValue("@ForumId", id);
                    deleteForumCmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction(nameof(ForumPage));
        }

        // GET: ForumController/EditComment/5
        public IActionResult EditComment(int commentId)
        {
            Comment comment = GetCommentById(commentId);
            return View(comment);
        }

        // POST: ForumController/EditComment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditComment(int commentId, Comment comment)
        {
            if (ModelState.IsValid)
            {
                // Add logic to update the comment in the database
                using (MySqlConnection connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();

                    string updateCommentQuery = "UPDATE comments SET UserName = @UserName, Content = @Content WHERE Id = @CommentId";
                    using (MySqlCommand updateCommentCmd = new MySqlCommand(updateCommentQuery, connection))
                    {
                        updateCommentCmd.Parameters.AddWithValue("@CommentId", commentId);
                        updateCommentCmd.Parameters.AddWithValue("@UserName", comment.UserName);
                        updateCommentCmd.Parameters.AddWithValue("@Content", comment.Content);

                        updateCommentCmd.ExecuteNonQuery();
                    }
                }

                return RedirectToAction(nameof(ForumPage));
            }

            return View(comment);
        }

        // GET: ForumController/DeleteComment/5
        public IActionResult DeleteComment(int commentId)
        {
            Comment comment = GetCommentById(commentId);
            return View(comment);
        }

        // POST: ForumController/DeleteComment/5
        [HttpPost, ActionName("DeleteComment")]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmDeleteComment(int commentId)
        {
            // Add logic to delete the comment from the database
            using (MySqlConnection connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                string deleteCommentQuery = "DELETE FROM comments WHERE Id = @CommentId";
                using (MySqlCommand deleteCommentCmd = new MySqlCommand(deleteCommentQuery, connection))
                {
                    deleteCommentCmd.Parameters.AddWithValue("@CommentId", commentId);
                    deleteCommentCmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction(nameof(ForumPage));
        }

        // Helper method to retrieve a forum by ID
        private Forum GetForumById(int forumId)
        {
            using (MySqlConnection connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                string forumQuery = "SELECT Id, Title, Description FROM forums WHERE Id = @ForumId";
                using (MySqlCommand forumCmd = new MySqlCommand(forumQuery, connection))
                {
                    forumCmd.Parameters.AddWithValue("@ForumId", forumId);

                    using (MySqlDataReader forumReader = forumCmd.ExecuteReader())
                    {
                        if (forumReader.Read())
                        {
                            return new Forum
                            {
                                Id = forumId,
                                Title = forumReader["Title"].ToString(),
                                Description = forumReader["Description"].ToString()
                            };
                        }
                    }
                }
            }

            return null;
        }

        // Helper method to retrieve a comment by ID
        private Comment GetCommentById(int commentId)
        {
            using (MySqlConnection connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                string commentQuery = "SELECT Id, UserName, Content FROM comments WHERE Id = @CommentId";
                using (MySqlCommand commentCmd = new MySqlCommand(commentQuery, connection))
                {
                    commentCmd.Parameters.AddWithValue("@CommentId", commentId);

                    using (MySqlDataReader commentReader = commentCmd.ExecuteReader())
                    {
                        if (commentReader.Read())
                        {
                            return new Comment
                            {
                                Id = commentId,
                                UserName = commentReader["UserName"].ToString(),
                                Content = commentReader["Content"].ToString()
                            };
                        }
                    }
                }
            }

            return null;
        }
    }
}
