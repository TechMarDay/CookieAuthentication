# CookieAuthentication
CookieAuthentication
https://techmarday.com/cookie-authentication-dotnet-core

Bài viết này mang tính chia sẻ nên có nhiều thiếu sót. Nếu có gì sai hoặc cần bổ sung mong các bạn giúp mình chỉnh sửa để tốt hơn. Thân.
Bài này mình sẽ sử dụng .net core 3.1. Bảng này microsoft LTS (Long term support) nên mình khuyến khích dùng.

Nói sơ qua về lý thuyết:

Authentication là gì?

Là xác thực user có quyền đăng nhập vào ứng dụng của bạn không.
Authorization là gì?  

Để phân quyền user sau khi đã đăng nhập vào ứng dụng
 

Liên hệ thực tế: Khi bạn dẫn em ny vào thuê phòng ở nhà nghỉ. Chủ nhà nghỉ sẽ yêu cầu chứng minh nhân dân (cái này là authentication) nếu bạn có chứng minh nhân dân sẽ được vào nhà nghỉ thuê phòng.
Sau đó chủ nhà nghỉ sẽ đưa cho bạn chìa khóa phòng cho riêng bạn (chìa khóa này không vào được phòng khác). Cái này là authorization.

Có thể dùng nhiều cách để authentication như cookie và token. Bài này mình sẽ nói về cookie.
Token các bạn có thể tham khảo ở đây:

https://techmarday.com/bai-viet/15/jwt-authentication-dotnet-core

Về cơ bản khi bạn login thì gửi username và password. Lúc này request tạo ra 1 cookie ở header. 

cookie này có thể thiết lập thời gian hết hạn. Các claims chứa các thông tin cần thiết tùy theo nhu cầu (role, username, age…)

Mỗi lần bạn vào 1 page thì sẽ check coi cookie này có hợp lệ không. Hợp lệ thì cho vào.

Khi logout thì sẽ xóa cookie cũ đi.

Demo:

Tạo mới project core web application

 



Chọn web application mvc controller



 

Bài này chỉ demo về authentication cookie nên mình sẽ không tạo database

Config trong startup

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(option =>
            {
                option.ExpireTimeSpan = TimeSpan.FromDays(1);
                option.LoginPath = "/login";
                option.AccessDeniedPath = "/login";
            }
            );

            services.AddMvc();
            services.AddRouting();
            services.AddSession();
Chỗ này khi unauthen và unauthor mình sẽ trả về trang login. Các bạn có thể thay bằng trang khác tùy chỉnh.

Cookie sẽ có thời hạn 1 ngày (ExpireTimeSpan)

Chỉnh mặc định vào trang login

            app.UseRouting();

            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Login}/{action=Index}/{id?}");
            });
Tạo user moel:

    public class UserModel
    {
        public string UserName { get; set; }

        public string PassWord { get; set; }

        public string Role { get; set; } //Should define enum constant
    }
Tạo login controller (mvc)



 

Lưu ý trang login phải là [AllowAnonymous]

[AllowAnonymous]
        public IActionResult Index(string userName, string password)
        {
            if (!string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(password))
            {
                return View();
            }

            ClaimsIdentity identity = null;
            bool isAuthenticated = false;
            UserModel user = null;

            //Chỗ này dự án thực tế sẽ query check trong database
            if (userName == "AdminTest" && password == "1234@")
                user = new UserModel
                {
                    UserName = "AdminTest",
                    PassWord = "1234@",
                    Role = "Admin" //Demo thử với quyền admin
                };
            if (user != null)
            {
                if (user.Role == "Admin")
                {
                    identity = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, userName),
                    new Claim(ClaimTypes.Role, "Admin"),
                    new Claim("userName", user.UserName),
                }, CookieAuthenticationDefaults.AuthenticationScheme);
                    isAuthenticated = true;
                }
            }


            if (isAuthenticated)
            {
                var principal = new ClaimsPrincipal(identity);

                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return Redirect("Home/Index");
            }

            return View();
        }
Chỗ này mình sẽ cho redirect sang trang Home nếu login thành công.

Add views:

<form class="form-signin" action="/login">
    <h1 class="h3 mb-3 font-weight-normal">Please sign in</h1>
    <label for="inputEmail" class="sr-only">UserName</label>
    <input type="text" id="username" name="username" class="form-control" placeholder="User Name" required autofocus>
    <br />
    <label for="inputPassword" class="sr-only">Password</label>
    <input type="password" id="password" name="password" class="form-control" placeholder="Password" required>
    <br />
    <button class="btn btn-lg btn-primary btn-block" type="submit">Sign in</button>
    <p class="mt-5 mb-3 text-muted">&copy; 2017-2020</p>
</form>
Vậy là cơ bản đã xong authentication cookie

Giờ mình sẽ test bằng cách thêm attribute vào index action ở home controller

     [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            return View();
        }
Test:

Nhập username:AdminTest

Password: 1234@ (Giống với username, password đã hard code trong login controller)



 

Thành công chuyển sang trang home

 



 

Test trường hợp login thất bại vẫn ở lại trang login do đã config ở startup
 

     option.LoginPath = "/login";
                option.AccessDeniedPath = "/login";
Nhập sai userName or password vẫn ở trang login

Hoặc ta có thể chỉnh ở home controller  thành Admin1. Sẽ không login được do sai role 
 

        [Authorize(Roles = "Admin1")]
        public IActionResult Index()
        {
            return View();
        }
 

Giờ ta sẽ thực hiện chức năng logout

Thêm button logout vào home page


<ul class="navbar-nav px-3 display-4">
    <li class="nav-item text-nowrap">
        <a class="nav-link" href="/logout">Sign out</a>
    </li>
</ul>
Add LogoutController

    [Authorize(Roles = "Admin")]
    public class LogoutController : Controller
    {
        public IActionResult Index()
        {
            var logout = HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/login");
        }
    }

Vậy là đã xong toàn bộ phần login và logout bằng authentication.
Bài viết này mang tính chia sẻ nên có nhiều thiếu sót. Nếu có gì sai hoặc cần bổ sung mong các bạn giúp mình chỉnh sửa để tốt hơn. Thân.

Tham khảo sourcecode ở github:

https://github.com/TechMarDay/CookieAuthentication
