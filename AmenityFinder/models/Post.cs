using System;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using AmenityFinder.utils;

namespace AmenityFinder.models
{
    public class Post: AbstractModel
    {
        public int Index { get; set; }
        public int Id { get; set; }
        public int Location { get; set; }
        public string Comment { get; set; }
        public float Rating { get; set; }
        public User User { get; set; }
        public bool IsAnonymous { get; set; }
        public int Upvotes { get; set; }
        public int Downvotes { get; set; }
        public DateTime Created { get; set; }

        public string GetUserName => User == null ? "Anonymous" : User.GetName();

        public BitmapImage GetUserPicture => User?.Picture == null ? new BitmapImage(new Uri("ms-appx:///Assets/anon_face.png")) : new BitmapImage(new Uri(User.Picture));

        public async Task<Post> Upvote()
        {
            var upvoteUri = string.Format(Constants.PostUpvoteUrl, Id);
            var httpResponseMessage = await Core.HttpUtils.MakeRequest(upvoteUri, null, HttpUtils.HttpMethod.Post);
            var responseData = await Core.HttpUtils.ProcessResponse(httpResponseMessage);

            var post = AbstractModel.Deserialize<Post>(responseData);
            Upvotes = post.Upvotes;
            Downvotes = post.Downvotes;
            IsAnonymous = post.IsAnonymous;
            Rating = post.Rating;
            Comment = post.Comment;
            return this;
        }

        public async Task<Post> Downvote()
        {
            var upvoteUri = string.Format(Constants.PostDownvoteUrl, Id);
            var httpResponseMessage = await Core.HttpUtils.MakeRequest(upvoteUri, null, HttpUtils.HttpMethod.Post);
            var responseData = await Core.HttpUtils.ProcessResponse(httpResponseMessage);

            var post = AbstractModel.Deserialize<Post>(responseData);
            Upvotes = post.Upvotes;
            Downvotes = post.Downvotes;
            IsAnonymous = post.IsAnonymous;
            Rating = post.Rating;
            Comment = post.Comment;
            return this;
        }
    }

    public class PaginatedPosts : AbstractPaginated<Post>
    {
        
    }

    public class PostResult
    {
        public bool Success { get; set; }
        public Post Result { get; set; }
    }

    public class NewPost : AbstractModel
    {
        public int Id { get; set; } = -1;
        public int Location { get; set; }
        public string Comment { get; set; }
        public float Rating { get; set; }
        public bool IsAnonymous { get; set; } = true;

        public async Task<Post> AddNewPost()
        {
            var url = Constants.PostListUrl;
            var method = HttpUtils.HttpMethod.Post;
            if (Id != -1)
            {
                url = string.Format(Constants.PostDetailUrl, Id);
                method = HttpUtils.HttpMethod.Put;
            }
            var jsonData = ToJson();
            var httpResponse =
                await Core.HttpUtils.MakeRequest(url, jsonData, method);
            var responseData = await Core.HttpUtils.ProcessResponse(httpResponse);
            var newPost = AbstractModel.Deserialize<Post>(responseData);
            return newPost;
        }
    }

}
