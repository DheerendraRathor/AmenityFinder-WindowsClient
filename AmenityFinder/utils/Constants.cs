using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmenityFinder.utils
{
    public class Constants
    {

        public const string KeyFile = "Resources";
        public const string MapServiceTokenKeyName = "MapServiceToken";
        public const string FbAppIdName = "FbAppId";

        public const string UserTokenName = "token-auth";
        public const string FbAccessTokenName = "access_token";
        public const int Timeout = 10000;

        public const string Scheme = "https";
        public const string BaseUrl = Scheme + "://" + "af.thecodershub.com/";
        public const string BaseApiUrl = BaseUrl + "api/";

        // Location URLs
        public const string LocationListUrl = BaseApiUrl + "location/";
        public const string LocationDetailUrl = LocationListUrl + "{0}/";
        public const string LocationGetPostsUrl = LocationDetailUrl + "get_posts/";
        public const string LocationFlagPostUrl = LocationDetailUrl + "flag_post/";
        public const string LocationSearchByBBoxUrl = LocationListUrl + "search_by_bbox/";

        // Post URLs
        public const string PostListUrl = BaseApiUrl + "post/";
        public const string PostDetailUrl = PostListUrl + "{0}/";
        public const string PostDownvoteUrl = PostDetailUrl + "downvote/";
        public const string PostUpvateUrl = PostDetailUrl + "/upvote/";

        //Account URLs
        public const string AccountListUrl = BaseApiUrl + "account/";
        public const string AccountLoginUrl = AccountListUrl + "login/";

    }
}
