# 🎵 MusicApp – Multi-Club Music Request Management System

## 📌 Overview

MusicApp is a cloud-based web application developed as an individual Final Year Project for a **BSc in Web Programming with Cyber Security**.

The system digitalizes and optimizes the song request process within nightclub environments by providing a secure, scalable, and responsive platform that connects **Users, DJs, and Administrators** in a unified system.

Built using **Blazor .NET 8 Server**, the application integrates external APIs, implements role-based authentication and authorization, and provides real-time request management alongside analytical dashboards.

---

## 🚀 Problem Statement

Traditional nightclub song request systems are often:

- Verbal or paper-based
- Unstructured and inefficient
- Lacking transparency
- Prone to duplicated or lost requests
- Without analytical insights

MusicApp solves these problems by offering a centralized, secure, and data-driven platform for managing music requests across multiple clubs.

---

## 🏗️ Technologies & Architecture

- **Frontend & Backend:** Blazor .NET 8 Server
- **Authentication & Authorization:** ASP.NET Identity Framework
- **Database:** Azure-hosted relational database
- **Hosting:** Microsoft Azure
- **External API Integration:** Spotify API
- **Data Visualization:** Blazorised Diagrams (Pie, Bar, Line charts)
- **Architecture Pattern:** Services → Controllers
- **Design:** Fully responsive (mobile-first)

---

## 👥 User Roles & Features

### 🎧 User

- Register and securely log in
- Scan QR code containing a `ClubID`
- Automatically navigate to the selected club
- Search songs dynamically via Spotify API
- Submit song requests
- View request status:
  - Pending
  - Accepted
  - Refused
- Re-request previously requested songs with one click
- Use a fully mobile-optimized interface

---

### 🎛️ DJ

- Assigned to specific clubs and performances by Admin
- View real-time incoming song requests
- Accept or reject requests
- Manage three request tables:
  - Pending requests
  - Accepted songs
  - Rejected songs
- View historical performance analytics:
  - 📊 Pie charts
  - 📈 Line charts
  - 📉 Bar charts
- Analyze previously requested songs for better music planning

---

### 🛠️ Admin

- Create, edit, and delete performances
- Create, edit, and delete users
- Assign DJs to clubs
- Generate dynamic analytical diagrams
- View statistics:
  - Per individual club
  - Across the entire database
- Full system-level access and management

---

## 📱 QR Code Club Navigation

Each club has a unique QR code containing its `ClubID`.

When scanned:
1. The user logs in (if not already authenticated)
2. The system automatically redirects to the correct club page

This ensures seamless in-club interaction.

---

## 🔐 Security Features

- Role-based access control (User, DJ, Admin)
- ASP.NET Identity authentication system
- Secure authorization policies
- Protected dashboards per role
- Azure cloud deployment
- Separation of concerns using Services & Controllers

---

## ☁️ Cloud Deployment

- Web application hosted on Microsoft Azure
- Database deployed on Azure
- Designed to support multiple clubs within one system
- Scalable cloud-based architecture

---

## 📊 Key Highlights

✔ Multi-club support  
✔ Real-time request management  
✔ Spotify API integration  
✔ Data analytics dashboards  
✔ Role-based security model  
✔ Cloud-native deployment  
✔ Mobile-first responsive design  

---

## 🎓 Academic Context

This project demonstrates practical implementation of:

- Modern web development using .NET 8
- Secure authentication & authorization mechanisms
- API integration (Spotify)
- Cloud deployment strategies
- Data visualization techniques
- Multi-role system architecture
- Cyber security principles in web applications

---

## 🔮 Future Improvements

- Real-time updates using SignalR
- Push notifications for request status updates
- Payment integration for premium song prioritization
- Advanced analytics with machine learning insights
- Admin activity logging & audit trails
