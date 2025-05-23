# Vai trò của ItemData, ItemView và ItemController

Dưới đây là bản mô tả ngắn gọn về cách ba thành phần này hoạt động theo mô hình MVC trong dự án *GoodSort*.

## ItemData (Model)
`ItemData` nằm trong thư mục **Model**. Ngoài các trường dữ liệu như ID và kiểu hiển thị (`ItemVisualType`), lớp này còn cung cấp một số logic cơ bản (ví dụ: trả về `Sprite` tương ứng từ `ItemIconConfig` và thuộc tính kiểm tra chế độ hiển thị). `ItemData` không trực tiếp xử lý việc vẽ hay tương tác UI, nhưng các lớp khác có thể dựa vào những thuộc tính này để hiển thị đúng thông tin.

## ItemView (View)
`ItemView` thuộc thư mục **View**. Script này quản lý hình ảnh item trên scene thông qua `SpriteRenderer` và chỉ xử lý việc hiển thị. Các thao tác kéo/thả và cập nhật dữ liệu đã được chuyển sang `ItemController`.

## ItemController (Controller)
`ItemController` nằm trong thư mục **Controller**. Nó đóng vai trò cầu nối giữa `ItemData` và `ItemView`: nhận dữ liệu từ model, điều khiển view cũng như xử lý tương tác kéo/thả. Mỗi khi một item được di chuyển sang kệ mới, `ItemController` gửi thông tin cập nhật tới `BoardController` để thay đổi trạng thái bảng.
