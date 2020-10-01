using System;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using Login.Models;
using System.Data.Entity;
using System.Web;

namespace Login
{
    [HubName("LoginChat")]
    public class GroupHub : Hub
    {
        
        private ApplicationDbContext dbsignalr = new ApplicationDbContext();

        //先判斷 user狀態 .第一次用 A.已聊天中 B.有空房間已進入房間並等人等人 C.等人 E.自己離開 F.對方離開 
        public string AccessCookie(string iCookie)
        {
            bool result = dbsignalr.Users.Where(x => x.ID == iCookie).Select(x => x.ID).Any();//判斷有沒有cookie

            //有Cookie 再判斷是甚麼狀態
            if (result == true)
            {
                Users sUser = dbsignalr.Users.First(c => c.ID == iCookie);
                string UserStatus = sUser.Status;

                if (UserStatus == "A")
                {
                    #region 已寫好 用在 狀況A
                    Users iUser = new Users();//jim_(note)這樣應該就是new一個新的entity
                    Users sUsers = dbsignalr.Users.First(c => c.ID == iCookie);

                    #region 把舊的group裡面的 clientid移除
                    string RoomID = sUsers.RoomID;//取出房號
                    string clientID = sUsers.ID;
                    Groups.Remove(clientID, RoomID);//移除group 舊的 connectionID
                    Groups.Add(Context.ConnectionId, RoomID);//並加入新的
                    #endregion

                    #region 重新存入 新的cookie 更新到 DB
                    iUser.ID = Context.ConnectionId;//新Cookie
                    iUser.Choose = sUsers.Choose;
                    iUser.RoomID = RoomID;
                    iUser.FirstCookie = sUser.FirstCookie;
                    iUser.Wait = "N";
                    iUser.Status = "A";
                    dbsignalr.Users.Add(iUser);
                    dbsignalr.SaveChanges();
                    #endregion

                    dbsignalr.Users.Remove(sUsers);//刪除舊的
                    dbsignalr.SaveChanges();

                    Rooms sRooms = dbsignalr.Rooms.First(c => c.ID == RoomID);
                    string Message = sRooms.Message;
                    if (sRooms.User1 == iCookie)
                    {
                        sRooms.User1 = Context.ConnectionId;//更新Cookie
                    }
                    else
                    {
                        sRooms.User2 = Context.ConnectionId;
                    }


                    dbsignalr.SaveChanges();


                    //或許要再創一個 移除cookie的前端涵式
                    Clients.Client(Context.ConnectionId).updateCookie(Context.ConnectionId);//到這邊結束前端 資料庫 cookie都更新好

                    #endregion

                    Clients.Client(Context.ConnectionId).addRoom(sRooms.ID, "A");
                    Clients.Client(Context.ConnectionId).showMessage("房間建立完成!");

                    #region 撈出之前的聊天對話
                    string SendMsg = sRooms.Message;
                    string FirstCookie = iUser.FirstCookie;
                    string[] MsgArray = SendMsg.Split(',');
                    Clients.Client(Context.ConnectionId).MsgReload(RoomID, FirstCookie, SendMsg);

                    #endregion

                }
                else if (UserStatus == "B")
                {
                    #region 已寫好 用在 狀況B
                    Users iUser = new Users();//jim_(note)這樣應該就是new一個新的entity
                    Users sUsers = dbsignalr.Users.First(c => c.ID == iCookie);//前一次的cookie

                    #region 把舊的group裡面的 clientid移除
                    string RoomID = sUsers.RoomID;//取出房號
                    string clientID = sUsers.ID;
                    Groups.Remove(clientID, RoomID);//移除group 舊的 connectionID
                    #endregion

                    #region 重新存入 新的cookie 更新到 DB
                    iUser.ID = Context.ConnectionId;//新Cookie
                    iUser.Choose = sUsers.Choose;
                    iUser.RoomID = RoomID;
                    iUser.FirstCookie = "A";
                    iUser.Wait = "Y";
                    iUser.Status = "B";
                    dbsignalr.Users.Add(iUser);
                    dbsignalr.SaveChanges();
                    #endregion

                    dbsignalr.Users.Remove(sUsers);//刪除舊的
                    dbsignalr.SaveChanges();

                    #region 更新Rooms的 User ID
                    Rooms sRooms = dbsignalr.Rooms.First(c => c.ID == RoomID);
                    string Message = sRooms.Message;
                    if (sRooms.User1 == iCookie)
                    {
                        sRooms.User1 = Context.ConnectionId;//更新Cookie
                    }
                    else
                    {
                        sRooms.User2 = Context.ConnectionId;
                    }


                    dbsignalr.SaveChanges();
                    #endregion

                    //或許要再創一個 移除cookie的前端涵式
                    Clients.Client(Context.ConnectionId).updateCookie(Context.ConnectionId);//到這邊結束前端 資料庫 cookie都更新好
                    //Clients.Client(Context.ConnectionId).LoginState("B");
                    #endregion

                    Clients.Client(Context.ConnectionId).addRoom(sRooms.ID, "B");
                    Clients.Client(Context.ConnectionId).showMessage("房間建立完成!");
                }
                else if (UserStatus == "C")
                {
                    #region 已寫好 用在 狀況C
                    Users iUser = new Users();//jim_(note)這樣應該就是new一個新的entity
                    Users sUsers = dbsignalr.Users.First(c => c.ID == iCookie);

                    #region 重新存入 新的cookie 更新到 DB
                    iUser.ID = Context.ConnectionId;//新Cookie
                    iUser.Choose = sUsers.Choose;
                    iUser.RoomID = "";
                    iUser.Wait = "N";
                    iUser.Status = "C";
                    dbsignalr.Users.Add(iUser);
                    dbsignalr.SaveChanges();
                    #endregion

                    dbsignalr.Users.Remove(sUsers);//刪除舊的
                    dbsignalr.SaveChanges();


                    //或許要再創一個 移除cookie的前端涵式
                    Clients.Client(Context.ConnectionId).updateCookie(Context.ConnectionId);//到這邊結束前端 資料庫 cookie都更新好

                    #endregion
                }
                else if (UserStatus == "F")
                {
                    Users iUser = new Users();//jim_(note)這樣應該就是new一個新的entity
                    Users sUsers = dbsignalr.Users.First(c => c.ID == iCookie);

                    #region 測試應該是成功的 重新存入 新的cookie 更新到 DB
                    iUser.ID = Context.ConnectionId;//新Cookie
                    iUser.Choose = sUsers.Choose;
                    iUser.RoomID = "";
                    iUser.Wait = "N";
                    iUser.Status = "C";
                    dbsignalr.Users.Add(iUser);
                    dbsignalr.SaveChanges();
                    #endregion

                    dbsignalr.Users.Remove(sUsers);//刪除舊的
                    dbsignalr.SaveChanges();
                    //要倒回 對方離開的畫面 並有離開的按鈕
                    Clients.Client(Context.ConnectionId).addRoom("", "F");
                    Clients.Client(Context.ConnectionId).showMessage("房間建立完成!");
                }
                return "F";
            }
            else
            {
                //是新使用者
                //DB 沒存Cookie 就會在ExistCookie 這一個function存 入Cookie
                return "N";
            }

        }

        //jim_(M)有沒有 cookie
        public void ExistCookie(string iChoose)
        {
            //bool UserID = dbsignalr.Users.Where(x => x.ID == iCookie && x.Wait =="N").Select(x => x.ID).Any();//判斷有沒有資料在裡面
            string iCookie = Context.ConnectionId;//之後有人離開之後 才有辦法再加入聊天室


            #region define Rooms db and db users
            Rooms iRooms = new Rooms();
            Users iUser = new Users();

            iUser.ID = iCookie;
            iUser.Choose = iChoose;
            #endregion

            //傳回去設定cookie
            Clients.Client(Context.ConnectionId).updateCookie(iCookie);

            string NewRoom = IsExistRoom(iCookie, iChoose);

            if (NewRoom == "Y")
            {

                iUser.Wait = NewRoom;
                iUser.Status = "C";
                iUser.RoomID = "";

                #region 存入人的資料
                //這邊不知道是不是一個就可以了
                dbsignalr.Users.Add(iUser);
                dbsignalr.SaveChanges();
                #endregion

                //輸出數量到前端
                int UserNum = dbsignalr.Users.Where(x => x.Wait == "Y" && x.RoomID == "").Select(x => x.ID).Count();
                Clients.Client(Context.ConnectionId).WaitRoom("聊天室已滿!!請等待" + UserNum + "人");//test_要等多少人
            }
            else
            {

                if (NewRoom != "NoUser1")
                {

                    //這邊撈出配對的人會有問題 看要不要 傳入配對方式進來
                    Users sUser1 = dbsignalr.Users.First(x => x.Choose == NewRoom && x.Wait == "Y");

                    iRooms.ID = sUser1.RoomID;
                    iRooms.User1 = sUser1.ID;//2020/07/08 改成房間ID等於第一個人 cookie當 房間建立起來之後房間名稱就固定
                    iRooms.User2 = iCookie;
                    iRooms.IsActivity = "Y";
                    iUser.RoomID = iRooms.ID;
                    iUser.Wait = "N";
                    iUser.Status = "A";
                    iUser.FirstCookie = "B";

                    //加進房間
                    Groups.Add(Context.ConnectionId, iRooms.ID);

                    #region 更新User1的狀態
                    Users uUser1 = dbsignalr.Users.First(c => c.RoomID == iRooms.ID);
                    uUser1.Wait = "N";
                    uUser1.Status = "A";
                    dbsignalr.SaveChanges();//成功這邊成功2020-06-10
                    #endregion

                    #region 更新前端房間狀態
                    Clients.Client(uUser1.ID).addRoomA(iRooms.ID, "A");//user1開始聊天
                    Clients.Client(Context.ConnectionId).addRoom(iRooms.ID, "A");//user2加進聊天
                    #endregion

                    #region 更新房間

                    dbsignalr.Entry(iRooms).State = EntityState.Modified;
                    dbsignalr.SaveChanges();//成功這邊成功2020-06-10
                    #endregion

                }
                else
                {


                    //這邊就是NoUsers的情況
                    iRooms.ID = iCookie;//這邊是update
                    iRooms.User1 = iCookie;
                    iRooms.User2 = "";
                    iRooms.Message = "";
                    iRooms.IsActivity = "N";
                    iUser.RoomID = iRooms.ID;
                    iUser.Wait = "Y";
                    iUser.Status = "B";
                    iUser.FirstCookie = "A";
                    Groups.Add(Context.ConnectionId, iRooms.ID);

                    #region
                    Clients.Client(Context.ConnectionId).addRoom(iRooms.ID, "B");//一個人進房間 並等待另一個人
                    Clients.Client(Context.ConnectionId).showMessage("房間建立完成!");
                    #endregion


                    #region 更新房間
                    dbsignalr.Rooms.Add(iRooms);
                    dbsignalr.SaveChanges();
                    //dbsignalr.Entry(iRooms).State = EntityState.Modified;
                    //dbsignalr.SaveChanges();//成功這邊成功2020-06-10
                    #endregion
                }

                #region 存入人的資料
                //這邊不知道是不是一個就可以了
                dbsignalr.Users.Add(iUser);
                dbsignalr.SaveChanges();
                #endregion

            }

        }

        //jim_(M)有沒有 空的房間
        public string IsExistRoom(string iCookie, string iChoose)
        {
            string NewRoom = "";
            int HaveUser = dbsignalr.Rooms.Where(x => x.User1 != "" && x.User2 == "").Select(x => x.ID).Count();
            //int EmptyRoom = dbsignalr.Rooms.Where(x => x.User1 == "" && x.User2 == "").Select(x => x.ID).Count();
            //哪先人可以跟你配對到
            //int MatchChoose = dbsignalr.Users.Where(x => x.Choose != iChoose && x.Wait == "Y" && x.RoomID != "").Select(x => x.ID).Count();
            int MatchChoose = 0;

            #region 判斷 你男生是要跟誰聊天 男男或是女生
            if (iChoose == "male,X")
            {
                //男 - 女
                MatchChoose = dbsignalr.Users.Where(x => x.Choose == "female,Y" && x.Wait == "Y" && x.RoomID != "").Select(x => x.ID).Count();
                NewRoom = "female,Y";
            }
            else if (iChoose == "male,Y")
            {
                //男 - 男
                MatchChoose = dbsignalr.Users.Where(x => x.Choose == "male,Y" && x.Wait == "Y" && x.RoomID != "").Select(x => x.ID).Count();
                NewRoom = "male,Y";
            }
            else if (iChoose == "female,Y")
            {
                //女 - 男
                MatchChoose = dbsignalr.Users.Where(x => x.Choose == "male,X" && x.Wait == "Y" && x.RoomID != "").Select(x => x.ID).Count();
                NewRoom = "male,X";
            }
            else if(iChoose == "female,X")
            {
                //女 - 女
                //這邊邏輯有錯 因為如果都是有配對到的 就應該要開心房間 單是這邊還是會友直所以是錯的
                MatchChoose = dbsignalr.Users.Where(x => x.Choose == "female,X" && x.Wait == "Y" && x.RoomID != "").Select(x => x.ID).Count();
                NewRoom = "female,X";
            }
            #endregion

            //這邊會優先判斷有沒有一個人的房間 有人 = true 才會進來
            if (HaveUser > 0)
            {
                //可以配對到的人 在哪一價room裡面
                if (MatchChoose > 0)
                {
                    //有一個人的房間
                }
                else
                {
                    //要自己建一個空房間
                    NewRoom = "NoUser1";
                }

            }
            else
            {
                //要自己建一個空房間
                NewRoom = "NoUser1";
            }


            return NewRoom;
        }

        //給第二次判斷用的
        public string GetEmptyRoom()
        {
            string NewRoom = "";
            bool HaveUser = dbsignalr.Rooms.Where(x => x.User1 == "" && x.User2 == "").Select(x => x.ID).Any();

            //這邊會優先判斷有沒有一個人的房間 有人 = true 才會進來
            if (HaveUser == true)
            {
                //有一個人的房間
                NewRoom = dbsignalr.Rooms.Where(x => x.User1 == "" && x.User2 == "").Select(x => x.ID).FirstOrDefault().ToString();

            }

            return NewRoom;
        }

        //建立房間的條件 要換成Room判別的條件
        //jim_(M)檢測有沒有房間有兩個人 如果進來的Cookie 是第二個Users的話 就可以開啟房間
        public void ActiveRoom()
        {
            string iCookie = Context.ConnectionId;
            //這邊要檢測 這個人有沒有在兩個人的房間裡面 有的畫 叫出房間的名稱
            bool Active = dbsignalr.Rooms.Where(x => (x.User1 != "" && x.User2 == iCookie) || (x.User1 == iCookie && x.User2 != "")).Select(x => x.ID).Any();
            //房間為空 就是在等待
            bool AddEmptyRoom = dbsignalr.Users.Where(x => x.ID == iCookie && x.Wait == "Y" && x.RoomID != "").Select(x => x.ID).Any();

            if (Active == true)
            {
                string RoomName = dbsignalr.Rooms.Where(x => (x.User1 != "" && x.User2 == iCookie) || (x.User1 == iCookie && x.User2 != "")).FirstOrDefault().ToString();
                Clients.Client(Context.ConnectionId).addRoom(RoomName);//如果有兩個人的話 就做這件事情
                Clients.Client(Context.ConnectionId).showMessage("房間建立完成!");
                //GetRoomList();
            }
            else
            {
                //如果那一個人沒有配對到，也還有空房間 沒有wait就會進來這裡
                if (AddEmptyRoom == true)
                {
                    //把一個人加進 到一個空的聊天室裡面
                    string RoomName = dbsignalr.Rooms.Where(x => x.User1 == iCookie).OrderBy(x => x.ID).Select(x => x.ID).FirstOrDefault().ToString();
                    Clients.Client(Context.ConnectionId).addRoom(RoomName);
                    Clients.Client(Context.ConnectionId).showMessage("房間建立完成!");
                    //GetRoomList();
                }

            }

        }

        /// <summary>
        /// 退出聊天室
        /// </summary>
        /// <param name="roomName"></param>
        public void RemoveFromRoom(string roomName)
        {
            Rooms iRooms = new Rooms();
            string strRoomID = "";
            string iCookie = Context.ConnectionId;//06-28前端沒改 應該是不用這行了

            //查詢房間是否存在
            bool RoomID = dbsignalr.Rooms.Where(x => x.User1 == iCookie || x.User2 == iCookie).Select(x => x.ID).Any();

            if (RoomID == true)
            {
                strRoomID = dbsignalr.Rooms.Where(x => x.User1 == iCookie || x.User2 == iCookie).Select(x => x.ID).FirstOrDefault().ToString();
                Rooms uRoom = dbsignalr.Rooms.First(c => c.ID == strRoomID);
                #region 刪除兩個人的cookie
                if (uRoom.User1 == iCookie)
                {
                    if (!string.IsNullOrEmpty(uRoom.User2))
                    {
                        string user2Cookie = uRoom.User2;
                        Users User2ID = dbsignalr.Users.First(x => x.ID == user2Cookie);
                        User2ID.Status = "F"; //如果user1離開的話 user2就紀錄 有人離開
                        dbsignalr.SaveChanges();
                    }


                    Users User1ID = dbsignalr.Users.First(x => x.ID == iCookie);
                    dbsignalr.Users.Remove(User1ID);
                    dbsignalr.SaveChanges();


                }
                else
                {
                    if (!string.IsNullOrEmpty(uRoom.User2))
                    {
                        string user1Cookie = uRoom.User1;
                        Users User1ID = dbsignalr.Users.First(x => x.ID == user1Cookie);
                        User1ID.Status = "F";
                        dbsignalr.SaveChanges();
                    }

                    Users User2ID = dbsignalr.Users.First(x => x.ID == iCookie);
                    dbsignalr.Users.Remove(User2ID);
                    dbsignalr.SaveChanges();

                }
                #endregion


                //存在則進入刪除
                if (!string.IsNullOrEmpty(strRoomID))
                {
                    #region 更新房間
                    /*uRoom.User1 = "";
                    uRoom.User2 = "";
                    uRoom.Message = "";
                    uRoom.IsActivity = "E";*/



                    //dbsignalr.Entry(uRoom).State = EntityState.Modified;
                    dbsignalr.Rooms.Remove(uRoom);//2020-07-08改成全部刪除
                    dbsignalr.SaveChanges();//成功這邊成功2020-06-10
                    #endregion


                    Groups.Remove(Context.ConnectionId, roomName);

                    //提示客戶端
                    Clients.Client(Context.ConnectionId).removeRoom("退出成功!");

                    if (uRoom.User1 == "" && uRoom.User2 == "")
                    {
                        bool WaitingUser = dbsignalr.Users.Where(c => c.RoomID == "" && c.Wait == "Y").Select(c => c.ID).Any();
                        if (WaitingUser == true)
                        {
                            ShowWaitNumAndReAction();
                        }

                    }
                }

            }
            else
            {
                //已經有人刪除
                Users sUser = dbsignalr.Users.First(c => c.ID == iCookie);
                dbsignalr.Users.Remove(sUser);
                dbsignalr.SaveChanges();
            }




        }




        //要丟等待數量給前端，同時 如果 有空房間出來就要執行一次
        public void ShowWaitNumAndReAction()
        {
            //Rooms uRooms = new Rooms();
            Users iUser = new Users();
            string NewRoom = "";
            bool bRoom = dbsignalr.Rooms.Where(c => c.IsActivity == "E").Select(c => c.ID).Any();

            //如果有多出空位 有人離開 就會執行這邊
            if (bRoom == true)
            {
                NewRoom = dbsignalr.Rooms.Where(x => x.User1 == "" && x.User2 == "").Select(x => x.ID).FirstOrDefault().ToString();
                Users sUser = dbsignalr.Users.First(c => c.RoomID == "" && c.Wait == "Y");
                sUser.RoomID = NewRoom;
                sUser.Wait = "Y";
                string ClientID = sUser.ID;

                dbsignalr.SaveChanges();
                //--------------------------------

                Rooms uRooms = dbsignalr.Rooms.First(c => c.ID == NewRoom);
                uRooms.User1 = sUser.ID;
                uRooms.User2 = "";
                uRooms.Message = "";
                uRooms.IsActivity = "N";
                //dbsignalr.Entry(uRooms).State = EntityState.Modified;//看改變class名稱可不可以
                dbsignalr.SaveChanges();
                //也可以在有Y的地方把 connectid跟iCookie存在一起
                //先做上面 上面可以在是下面

                Groups.Add(ClientID, NewRoom);//不會是那一個沒位子 的人頁面 觸發的 所以這邊會沒值 所以需要多存這一筆connectid
                Clients.Client(ClientID).addRoom(NewRoom);
                Clients.Client(ClientID).removeRoom("終於進房拉");
            }


        }

        /// <summary>
        /// 給分組內所有的使用者傳送訊息
        /// </summary>
        /// <param name="Room">分組名</param>
        /// <param name="Message">資訊</param>
        public void SendMessage(string Room, string Message)
        {
            //Group 沒有連線到 context.connectid頁面
            Rooms uRooms = dbsignalr.Rooms.First(c => c.ID == Room);

            string User1 = uRooms.User1;
            string User2 = uRooms.User2;

            //這邊可以嘗試 不同的 js跑他們的輸出 這樣就不用船AB變數到前端
            if (Context.ConnectionId == User1)
            {
                uRooms.Message += "A" + Message + ",";
                Message = "A" + Message;

            }

            if (Context.ConnectionId == User2)
            {
                uRooms.Message += "B" + Message + ",";
                Message = "B" + Message;
            }

            dbsignalr.SaveChanges();
            Clients.Group(Room, new string[0]).sendMessageA(Room, Context.ConnectionId, Message);
            //Clients.Group(Room, new string[0]).sendMessage(Room, Message + " " + DateTime.Now.ToString("HH:mm:ss"));
        }
    }
}