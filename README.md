# Project Paint - Lập trình Windows - 21_3
### Thành viên

|       Họ và tên      |   MSSV   |   Điểm đề nghị|   
|----------------------|:--------:|:-------:      |
| Nguyễn Thị Thanh Hoa | 21120071 |	   10         |
| Nguyễn Thanh Huệ     | 21120076 |	   10            |
| Nguyễn Hà Hạnh Giang | 21120237 |	   10            |


## Hướng dẫn chạy lần đầu:

1. Clean solution -> Build/Rebuild solution
2. Vào thư mục \Paint\LineLib\bin\Debug\net7.0-windows -> Copy file **LineLib.dll** -> Paste sang thư mục \Paint\Paint\bin\Debug\net6.0-windows
***Nếu  trong thư mục .\Paint\Paint\bin\Debug\net7.0-windows có file LineLib.dll rồi thì xóa file đó đi và thực hiện copy như trên***
3. Làm tương tự với ElippseLib, HeartLib, RectangleLib.


## Basic graphic object
- Line: controlled by two points, the starting point, and the endpoint
- Rectangle: controlled by two points, the left top point, and the right bottom point
- Ellipse: controlled by two points, the left top point, and the right bottom point

## Các chức năng đã làm được
### `Core Requirements:` hoàn thành 100%
**1. Dynamically load all graphic objects that can be drawn from external DLL files.**

**2. The user can choose which object to draw.**

**3. The user can see the preview of the object they want to draw.**

**4. The user can finish the drawing preview and their change becomes permanent with previously drawn objects.**

**5. The list of drawn objects can be saved and loaded again for continuing later in self-defined binary format**

**6. Save and load all drawn objects as an image in bmp/png/jpg format (rasterization).**

### `Improvements`
**7. Used Fluent.Ribbon to obtain MS Paint-like user interface.**

**8. A bonus shape for the user to create: heart.**

**9. Create new canvas and the application will of course ask the user for confirmation if any new changes is detected.**

**10. Allow the user to change the color, pen width, stroke type (dash, dot, dash dot dot...)**

**11. Adding image to the canvas.**

**12. Zooming**

**13. Cut / Copy / Paste**

**14. Undo, Redo**

**15. Fill color by boundaries**

**16. Objects in drawable list have their own icons instead of plain text.**

**17.Shift mode turns a rectangle into a square, an ellipse into a circle instead of adding graphic object square and circle.**

## 🕑 Thời gian làm việc

| Task name | Hours |
| ------------- | --- |
| Core Requirements - 1 | 2 |
| Core Requirements - 2 | 2 |
| Core Requirements - 3 | 2 |
| Core Requirements - 4 | 2 |
| Core Requirements - 5 | 3 |
| Core Requirements - 6 | 3 |
| Improvements - 7 | 4 |
| Improvements - 8 | 4 |
| Improvements - 9 | 1 |
| Improvements - 10 | 4 |
| Improvements - 11 | 1 |
| Improvements - 12 | 4 |
| Improvements - 13 | 4 |
| Improvements - 14 | 3 |
| Improvements - 15 | 3|
| Improvements - 16 | 1 |
| Improvements - 17 | 3 |
| **SUM** | **46** |


## Điểm đề nghị 

**10 điểm**

## Link video demo




