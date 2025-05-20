# Tài Liệu Thiết Kế Game (Game Design Document - GDD)

**Tên Game (Dự kiến):** Goodsort
**Phiên bản GDD:** 8.1 (Chi tiết hóa - Mô hình Stack & Sinh Level Động)
**Ngày:** 16 tháng 4 năm 2025

## Mục Lục

1.  Giới Thiệu & Tầm Nhìn Sản Phẩm
    * 1.1. Giới thiệu
    * 1.2. Tầm nhìn
    * 1.3. Mục tiêu chính
    * 1.4. Đối tượng người chơi mục tiêu
    * 1.5. Điểm Bán Hàng Độc Đáo (USP)
2.  Tổng Quan Về Game
    * 2.1. Thể Loại
    * 2.2. Nền tảng
    * 2.3. Concept Cốt Lõi
    * 2.4. Vòng Lặp Gameplay Cốt Lõi
3.  Cơ Chế Gameplay Chi Tiết
    * 3.1. Bàn Chơi: Lưới & Stack Item Nội Bộ
    * 3.2. Vật Phẩm (Items)
    * 3.3. Luật Di Chuyển
    * 3.4. Luật Ghép Bộ (Matching) & Lộ Diện Item Mới
    * 3.5. Vật Cản (Obstacles) & Bộ Đếm
    * 3.6. Hệ Thống Combo
    * 3.7. Tiền Tệ (Coins) & Cơ Chế Thêm Thời Gian
4.  Hệ Thống Thưởng & Tăng Cường Gắn Kết
    * 4.1. Phần Thưởng Tỷ Lệ Biến Đổi (Variable Ratio Rewards)
    * 4.2. Thưởng Chuỗi (Streak Bonuses: Daily & Winning)
    * 4.3. Hiệu Ứng Suýt Soát (Near-Miss Effect)
5.  Thiết Kế Level, Sinh Level Thủ Tục
    * 5.1. Triết Lý Thiết Kế Level: Thích ứng & Luôn Mới Mẻ
    * 5.2. Cấu Trúc Level: Khuôn Mẫu Tham Số Hóa (LevelTemplateSO)
    * 5.3. Quy Trình Sinh Level Thủ Tục (Khi Bắt Đầu Level)
    * 5.4. Đảm Bảo Tính Khả Giải: Kiểm Tra & Xử Lý
6.  Điều Kiện Thắng/Thua
7.  Giao Diện Người Dùng (UI) & Trải Nghiệm Người Chơi (UX)
8.  Định Hướng Mỹ Thuật & Âm Thanh
9.  Chiến Lược Thương Mại Hóa (Monetization)
10. Cân Nhắc Kỹ Thuật (Từ Góc Độ Design)
11. Phạm Vi Phiên Bản Đầu (MVP) & Tiềm Năng Phát Triển

## 1. Giới Thiệu & Tầm Nhìn Sản Phẩm

### 1.1. Giới thiệu
Chào mừng đến với "Goodsort", một làn gió mới trong thể loại game giải đố trên di động. Thay vì cách chơi xếp lớp truyền thống, "Goodsort" giới thiệu cơ chế độc đáo: mỗi ô trên bàn chơi chứa một "chồng" vật phẩm ẩn, đòi hỏi người chơi vừa phải khéo léo sắp xếp, vừa phải tính toán để khám phá những gì ẩn chứa bên trong.

### 1.2. Tầm nhìn
Chúng tôi mong muốn "Goodsort" trở thành một game giải đố "phải có" trên điện thoại của người chơi, mang lại những giờ phút thư giãn nhưng đầy thử thách trí tuệ. Tầm nhìn của chúng tôi là một trò chơi không bao giờ cũ, luôn tạo ra sự hứng thú và cảm giác chinh phục được cá nhân hóa.

### 1.3. Mục tiêu chính
Thu hút và giữ chân người chơi bằng gameplay có chiều sâu, luôn mới mẻ; tối ưu hóa trải nghiệm thông qua cá nhân hóa độ khó; xây dựng một sản phẩm chất lượng cao, dễ mở rộng và có tiềm năng thương mại tốt.

### 1.4. Đối tượng người chơi mục tiêu
Những người yêu thích giải đố, từ người chơi phổ thông (casual) muốn thư giãn đến người chơi có kinh nghiệm hơn (mid-core) tìm kiếm thử thách logic và không gian mới lạ. Đặc biệt thu hút những người chơi thích sự đa dạng và muốn một trải nghiệm phù hợp với trình độ của mình.

### 1.5. Điểm Bán Hàng Độc Đáo (USP)
* **Gameplay Stack Item Độc Đáo:** Khác biệt với cơ chế layer thông thường.
* **Vô Hạn Level:** Sinh level thủ tục tạo ra sự đa dạng không giới hạn.
* **Độ Khó Thích Ứng (REM):** Game tự điều chỉnh để phù hợp với trình độ của bạn.
* **Thử Thách Obstacle & Combo:** Yếu tố chiến thuật và kỹ năng.
* **Thiết Kế Thân Thiện:** Đồ họa hấp dẫn, dễ tiếp cận.

## 2. Tổng Quan Về Game

### 2.1. Thể Loại
**Thể Loại:** Puzzle (Match-3 / Sorting lai, Procedural Generation, Adaptive Difficulty, Time-Management).

### 2.2. Nền tảng
**Nền tảng:** Mobile (iOS & Android), phát triển bằng Unity Engine.

### 2.3. Concept Cốt Lõi
Người chơi tương tác với 3 vật phẩm (items) hiển thị trong mỗi ô lớn (Large Cell) trên một bàn chơi dạng lưới (Grid). Mục tiêu là di chuyển các item vào các ô trống để tạo thành bộ 3 item giống nhau trong cùng một ô lớn. Khi ghép thành công (match), 3 item đó biến mất và 3 item tiếp theo từ "kho dự trữ" (stack) ẩn bên trong chính ô đó sẽ lộ diện. Vượt qua các vật cản (Obstacles) có bộ đếm và hoàn thành mục tiêu (thường là dọn sạch tất cả item) trước khi hết giờ. Độ khó và cấu trúc level được sinh tự động và cá nhân hóa bởi hệ thống REM.

### 2.4. Vòng Lặp Gameplay Cốt Lõi (Người chơi làm gì?)
* **Nhìn:** Xem item nào đang hiện, ô nào trống, vật cản nào còn bao nhiêu "máu" (bộ đếm), còn bao nhiêu thời gian.
* **Nghĩ:** "Mình nên dọn ô nào trước?", "Lấy item nào đi đâu để tạo match?", "Làm sao để phá vật cản kia?".
* **Chạm:** Chọn một item, chọn một ô trống hợp lệ để chuyển đến.
* **Xem Kết Quả:**
    * **Tạo Match!** -> Item biến mất, hiệu ứng vui mắt, vật cản yếu đi, có thể được thưởng Coins, 3 item mới xuất hiện từ chính ô đó. Combo tăng!
    * **Chưa Match...** -> Item chỉ di chuyển, combo (nếu có) bị ngắt.
    * **Dọn sạch ô!** -> Di chuyển hết item ra khỏi ô -> 3 item mới từ stack của ô đó xuất hiện.
    * **Stack hết!** -> Ô đó trở thành ô trống vĩnh viễn, có thể dùng làm "kho chứa" tạm.
* **Chơi Tiếp:** Lặp lại Nhìn -> Nghĩ -> Chạm cho đến khi thắng hoặc thua. Hệ thống REM âm thầm theo dõi và điều chỉnh cho lần chơi sau hoặc màn chơi kế tiếp.

## 3. Cơ Chế Gameplay Chi Tiết

### 3.1. Bàn Chơi: Lưới & Stack Item Nội Bộ
* **Lưới (Grid):** Bàn chơi được chia thành nhiều Ô Lớn (Large Cells), ví dụ 5x4, 6x5... (kích thước có thể thay đổi theo level).
* **Ô Lớn (Large Cell):** Mỗi Ô Lớn là đơn vị cơ bản, chứa:
    * **3 Ô Nhỏ (Small Slots):** Nằm ngang, là nơi hiển thị tối đa 3 item và là nơi người chơi tương tác chính (chạm, di chuyển).
    * **Stack Item Ẩn (List<int>):** Đây là điểm khác biệt chính. Mỗi ô nhỏ có một danh sách (List) riêng chứa các item được xếp chồng lên nhau theo thứ tự. Item ở cuối danh sách là item "sâu" nhất. Số lượng item trong danh sách này quyết định "độ sâu" của ô đó. Người chơi không nhìn thấy trực tiếp danh sách này.
* **Trạng thái Ô Nhỏ:** Một Ô Nhỏ có thể: chứa 1 item (lấy từ đầu List ẩn), hoặc trống (nếu stack ẩn đã hết hoặc item đã bị di chuyển đi).
* **Ô Trống Vĩnh Viễn:** Khi List ẩn của một Ô Lớn cạn kiệt, 3 Ô Nhỏ của nó sẽ trở thành ô trống vĩnh viễn, hoạt động như những ô trống thông thường khác trên bàn chơi.

### 3.2. Vật Phẩm (Items)
* Nhiều loại với hình ảnh/màu sắc riêng biệt, dễ phân biệt (ví dụ: Bánh quy, Kẹo, Trái cây...).
* Số lượng và loại item được sử dụng sẽ thay đổi tùy theo level (định nghĩa trong LevelTemplateSO).

### 3.3. Luật Di Chuyển
* Chỉ có thể chọn 1 item đang hiển thị trong Ô Nhỏ.
* Chỉ có thể di chuyển item đó đến 1 Ô Nhỏ khác đang trống.
* Không thể hoán đổi vị trí 2 item.
* Không thể di chuyển item vào Ô Lớn đang bị Obstacle che phủ.

### 3.4. Luật Ghép Bộ (Matching) & Lộ Diện Item Mới
* **Điều kiện Match:** Phải có 3 item giống hệt nhau nằm trong 3 Ô Nhỏ hiển thị của cùng một Ô Lớn.
* **Khi Match thành công:**
    * **Xóa Item:** 3 item vừa match sẽ biến mất khỏi 3 Ô Nhỏ. (Có hiệu ứng hình ảnh/âm thanh).
    * **Giảm Bộ Đếm Obstacle:** Bộ đếm của tất cả Obstacle trên bàn chơi giảm đi 1.
    * **Xử lý Combo:** Bắt đầu hoặc tăng bộ đếm combo.
    * **Thưởng Coins (Có thể):** Có cơ hội nhận Coins dựa trên mốc combo (xem Mục 4.1).
    * **Lộ Diện Item Mới (Từ Stack):** Hệ thống lấy tối đa 3 item từ đầu danh sách ẩn (hiddenItems) của chính Ô Lớn vừa match. Các item này được đặt vào 3 Ô Nhỏ vừa trống. Nếu danh sách ẩn không đủ 3 item, các ô còn lại sẽ trống. Nếu danh sách ẩn hết sạch, ô đó trở thành trống vĩnh viễn.
* **Lộ Diện Khi Dọn Sạch Ô Hiển Thị:** Nếu người chơi di chuyển cả 3 item ra khỏi Ô Nhỏ hiển thị của một Ô Lớn (làm chúng trống) VÀ danh sách ẩn của ô đó vẫn còn item -> Lộ diện tối đa 3 item tiếp theo từ danh sách ẩn vào Ô Nhỏ.

### 3.5. Vật Cản (Obstacles) & Bộ Đếm
* Xuất hiện ngẫu nhiên (dựa trên template) trên một số Ô Lớn khi bắt đầu level.
* **Chức năng:** Che phủ hoàn toàn 3 Ô Nhỏ, ngăn chặn mọi tương tác (không di chuyển item vào/ra được, không thể match trong ô đó, không thể lộ diện item mới từ stack của ô đó).
* **Bộ Đếm (Counter):** Mỗi Obstacle có một con số hiển thị (ví dụ: 3, 4, 5). Con số này cho biết cần bao nhiêu lần match ở các ô khác để phá hủy nó.
* **Giảm Bộ Đếm:** Mỗi khi người chơi tạo match thành công ở bất kỳ Ô Lớn nào khác trên bàn, bộ đếm của tất cả các Obstacle đang hoạt động sẽ giảm đi 1. (Có hiệu ứng số nhảy).
* **Phá Hủy:** Khi bộ đếm về 0, Obstacle biến mất, Ô Lớn đó trở lại hoạt động bình thường.
* **REM Bias:** Hệ thống REM có thể giới hạn giá trị bộ đếm tối đa được sinh ra cho một lượt chơi (ví dụ: nếu người chơi đang gặp khó, level chỉ sinh obstacle có bộ đếm tối đa là 4 thay vì 5).
* **Tùy chọn:** Có thể cho phép xem Quảng cáo Thưởng để phá hủy ngay 1 Obstacle.

### 3.6. Hệ Thống Combo
* Đếm số lần match thành công liên tiếp.
* Có một khoảng thời gian ngắn (ví dụ: 5 giây, cần cân bằng) sau mỗi lần match để thực hiện match tiếp theo và duy trì combo.
* Combo bị ngắt nếu: hết thời gian chờ hoặc người chơi di chuyển item mà không tạo thành match.

### 3.7. Tiền Tệ (Coins) & Cơ Chế Thêm Thời Gian
* **Coins:** Đơn vị tiền tệ chính, kiếm được chủ yếu từ Thưởng Combo và Thưởng Chuỗi.
* **Thêm Thời Gian:** Khi thời gian sắp hết, người chơi có thể chọn:
    * Dùng Coins để mua thêm một khoảng thời gian (ví dụ: +15 giây).
    * Xem Quảng cáo Thưởng (Rewarded Ad) để nhận thêm thời gian.
* Chi phí Coins, lượng thời gian cộng thêm, và giới hạn số lần thực hiện cần được cân bằng cẩn thận.

## 4. Hệ Thống Thưởng & Tăng Cường Gắn Kết

### 4.1. Phần Thưởng Tỷ Lệ Biến Đổi (Variable Ratio Rewards)
* **Mục đích:** Tạo sự bất ngờ và hứng thú, giống như "mở hộp quà".
* **Áp dụng:** Khi người chơi đạt các mốc combo (ví dụ: 3, 5, 7...), lượng Coins thưởng sẽ không cố định mà dao động ngẫu nhiên trong một khoảng nhỏ (ví dụ: mốc 5 combo có thể nhận 8, 9, 10, 11 hoặc 12 Coins thay vì luôn là 10). Hệ thống REM có thể ảnh hưởng nhẹ đến tỷ lệ nhận được phần thưởng cao hơn nếu người chơi đang có dấu hiệu chán nản.

### 4.2. Thưởng Chuỗi (Streak Bonuses)
* **Mục đích:** Khuyến khích sự đều đặn và kỹ năng.
* **Daily Streak:** Thưởng Coins/item hỗ trợ khi người chơi vào game mỗi ngày liên tục. Phần thưởng tăng dần.
* **Winning Streak:** Thưởng thêm (ví dụ: bonus Coins ở level tiếp theo) khi người chơi thắng nhiều level liên tiếp mà không thua.

### 4.3. Hiệu Ứng Suýt Soát (Near-Miss Effect)
* **Mục đích:** Giảm cảm giác bực bội khi thua sát nút, tạo động lực chơi lại.
* **Áp dụng:** Khi thua do hết giờ nhưng chỉ còn rất ít item hoặc có obstacle sắp vỡ (counter=1), màn hình thua sẽ có thông điệp đặc biệt ("Chút nữa thôi!", "Sắp phá được rồi!") và có thể có hiệu ứng hình ảnh/âm thanh thể hiện sự tiếc nuối. Đây cũng là một tín hiệu cho hệ thống REM biết người chơi đã tiến rất gần.

## 5. Thiết Kế Level, Sinh Level Thủ Tục

### 5.1. Triết Lý Thiết Kế Level: Thích ứng & Luôn Mới Mẻ
* Chúng ta không tạo ra hàng trăm level cố định. Thay vào đó, chúng ta tạo ra các Khuôn Mẫu (Templates) cho từng mốc level.
* Hệ thống sẽ tự động sinh ra (generate) một phiên bản level duy nhất cho mỗi lượt chơi dựa trên Template đó và điều chỉnh theo người chơi thông qua REM.
* Mục tiêu là tạo ra vô vàn thử thách khác nhau nhưng vẫn đảm bảo một đường cong độ khó tổng thể hợp lý và người chơi luôn cảm thấy phù hợp với trình độ của mình (trạng thái "flow").

### 5.2. Cấu Trúc Level: Khuôn Mẫu Tham Số Hóa (LevelTemplateSO)
* Mỗi "số hiệu level" (ví dụ: Level 10-15 thuộc nhóm "Trung Bình") sẽ sử dụng một hoặc một vài LevelTemplateSO.
* **Template là gì?** Là một file cấu hình (ScriptableObject trong Unity) chứa các quy tắc và khoảng giá trị chứ không phải layout cố định. Ví dụ:
    * `gridSize`: Khoảng 5x4 đến 6x5.
    * `cellDepthConfig`: Quy tắc về độ sâu các ô (ví dụ: đa số sâu 3-5 item, một vài ô sâu 7).
    * `allowedItemTypes`: Chỉ dùng item A, B, C, D.
    * `itemSpawnRules`: Tổng số mỗi loại item (bội số 3).
    * `obstacleConfig`: Tỷ lệ xuất hiện obstacle 15-25%, khoảng bộ đếm 3-5.
    * `initialEmptyVisibleSlotsRange`: Khoảng 3-6 ô trống ban đầu.
    * `difficultyHints`: "Medium".
    * `generationRules`: Các luật chống bẫy cơ bản.

### 5.3. Quy Trình Sinh Level Thủ Tục (Khi Bắt Đầu Level)
* **Yêu cầu:** Phải chạy bất đồng bộ (async) để không làm treo game, cần có chỉ báo loading.
* **Các bước chính:**
    1.  **Load Template & Nhận Input REM:** LevelManager tải LevelTemplateSO phù hợp. Lấy thông tin người chơi từ REM (REMInputParams: `failStreak`, `avgCompletionTimeLastN`, `winRateLastN` - có thể mở rộng).
    2.  **Điều Chỉnh Tham Số Mục Tiêu (Bias):** ProceduralLevelGenerator dựa vào REMInputParams để chọn các giá trị mục tiêu cụ thể trong khoảng cho phép của Template. Ví dụ quan trọng: Nếu `failStreak` cao, nó sẽ giảm giới hạn trên của bộ đếm obstacle có thể sinh ra (ví dụ: chỉ sinh tối đa 4 thay vì 5) và có thể tăng nhẹ số ô trống mục tiêu.
    3.  **Tạo Quỹ Item Tổng:** Dựa trên `itemSpawnRules`.
    4.  **Phân Bổ Lớp Hiển Thị:** Đặt các item đầu tiên lên các ô nhỏ hiển thị, đảm bảo có nước đi ban đầu (CheckInitialMoves).
    5.  **Phân Bổ Stack Ẩn:** Đưa item còn lại vào các `List<int>` ẩn của từng ô, áp dụng các luật cơ bản để tránh bẫy (ví dụ: không dồn hết item cần phá obstacle vào chính stack bị chặn đó).
    6.  **Đặt Obstacle:** Sinh obstacle với bộ đếm trong khoảng đã được điều chỉnh (bias).

### 5.4. Đảm Bảo Tính Khả Giải: Kiểm Tra & Xử Lý
* **Kiểm Tra Heuristic:** Sau khi ProceduralLevelGenerator tạo xong cấu trúc dữ liệu level (GeneratedLevelData), SolvabilityChecker sẽ chạy các kiểm tra thiết yếu:
    * Đủ item mỗi loại (bội số 3)?
    * Có nước đi ban đầu không?
    * Tổng số match tiềm năng có đủ để phá hết obstacle không?
    * Có bị bẫy obstacle tự chặn không (kiểm tra cơ bản)?
    * Có nguy cơ hết ô trống không (kiểm tra cơ bản)?
* **Cơ Chế Sinh Lại:** Nếu bất kỳ kiểm tra nào thất bại, hệ thống sẽ tự động thử sinh lại level với một seed ngẫu nhiên khác (giới hạn số lần, ví dụ: 5 lần).
* **Cơ Chế Dự Phòng:** Nếu sau 5 lần thử vẫn thất bại (rất hiếm nếu thuật toán tốt), LevelManager sẽ load một level "cứu cánh" tĩnh, đơn giản, đã được tạo sẵn để đảm bảo người chơi luôn có thể tiếp tục.

## 6. Điều Kiện Thắng/Thua
* **Thắng:** Dọn sạch tất cả item trong tất cả các stack của mọi Ô Lớn trên bàn chơi.
* **Thua:** Hết thời gian quy định VÀ người chơi không sử dụng tùy chọn thêm giờ (Coins/Ad). Trạng thái Near-Miss sẽ được ghi nhận nếu có.

## 7. Giao Diện Người Dùng (UI) & Trải Nghiệm Người Chơi (UX)
* **UI Chính:** Hiển thị rõ ràng bàn chơi, item, ô trống, obstacle và bộ đếm, thời gian còn lại, số Coins.
* **Phản hồi:** Cần đầu tư vào hiệu ứng hình ảnh (visual effects - VFX) và âm thanh (sound effects - SFX) cho các hành động: chọn item, di chuyển, match thành công, combo tăng, item mới lộ diện, obstacle giảm số/bị phá, hết giờ, thắng/thua, near-miss...
* **Loading:** Bắt buộc phải có chỉ báo loading rõ ràng và mượt mà trong quá trình sinh level bất đồng bộ giữa các màn chơi.

## 8. Định Hướng Mỹ Thuật & Âm Thanh
* **Mỹ thuật:** Phong cách sáng sủa, màu sắc hài hòa, thân thiện. Item và Obstacle cần có thiết kế độc đáo, dễ nhận biết. Ưu tiên sự rõ ràng và dễ nhìn. Giao diện người dùng (UI) sạch sẽ, hiện đại.
* **Âm thanh:** Nhạc nền (BGM) tạo cảm hứng, có thể tăng tiết tấu khi gần hết giờ. SFX cần "thỏa mãn" (satisfying) cho các hành động match, combo, phá obstacle. Âm thanh UI rõ ràng.

## 9. Chiến Lược Thương Mại Hóa (Monetization)
* **Mô hình chính (MVP):** Quảng cáo Thưởng (Rewarded Ads). Người chơi có thể chọn xem quảng cáo để nhận lợi ích trực tiếp (thêm thời gian, phá obstacle, nhận Coins...).
* **Nguyên tắc:** Không làm gián đoạn trải nghiệm cốt lõi. Quảng cáo là tùy chọn của người chơi.
* **Tương lai:** Có thể xem xét thêm Gói Coins (In-App Purchase - IAP), loại bỏ quảng cáo (IAP).

## 10. Cân Nhắc Kỹ Thuật (Từ Góc Độ Design)
* Mô hình Stack/Array: Thay đổi cách tư duy về "độ sâu" và liên kết không gian.
* Sinh Level Thủ Tục: Yêu cầu sự hợp tác chặt chẽ giữa Design và Dev để tạo Template và Luật hợp lý. Playtesting liên tục là bắt buộc.
* Loading Bất Đồng Bộ: Là yêu cầu thiết kế để đảm bảo UX tốt.
* Lưu trữ Stack: Đã chốt sử dụng `List<int>`.

## 11. Phạm Vi Phiên Bản Đầu (MVP) & Tiềm Năng Phát Triển

* **MVP Scope:**
    * Gameplay cốt lõi hoàn chỉnh với mô hình Stack/Array.
    * Hệ thống sinh level thủ tục dựa trên vài LevelTemplateSO cơ bản (tạo thủ công template).
    * Thuật toán sinh "Total Pool + Basic Rules".
    * Bộ kiểm tra Heuristic cơ bản + Cơ chế Sinh lại + Fallback level tĩnh.
    * Hệ thống thưởng Variable Ratio Combo, Daily Streak cơ bản.
    * Giao diện cơ bản, đủ chức năng. Async loading.
    * Analytics & Ads cơ bản.
* **Ưu tiên:** Ổn định, logic cốt lõi đúng, hệ thống sinh level hoạt động tin cậy.