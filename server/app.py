from flask import Flask, request, jsonify, render_template, redirect, url_for, flash, session
from flask_sqlalchemy import SQLAlchemy
from flask_cors import CORS
from datetime import datetime
from werkzeug.security import generate_password_hash, check_password_hash
from functools import wraps
import os
import platform
import psutil
import json
import uuid

app = Flask(__name__, template_folder='templates', static_folder='static')
CORS(app)

# Configuration
app.config['SQLALCHEMY_DATABASE_URI'] = 'sqlite:///photostudio.db'
app.config['SQLALCHEMY_TRACK_MODIFICATIONS'] = False
app.config['SECRET_KEY'] = os.environ.get('SECRET_KEY', 'default-dev-key-change-in-production')
db = SQLAlchemy(app)

# Models
class User(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    username = db.Column(db.String(100), nullable=False, unique=True)
    email = db.Column(db.String(100))
    device_id = db.Column(db.String(100))
    last_login = db.Column(db.DateTime)
    created_at = db.Column(db.DateTime, default=datetime.utcnow)
    activities = db.relationship('UserActivity', backref='user', lazy=True)

class UserActivity(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    user_id = db.Column(db.Integer, db.ForeignKey('user.id'), nullable=False)
    action = db.Column(db.String(100), nullable=False)
    timestamp = db.Column(db.DateTime, default=datetime.utcnow)
    details = db.Column(db.Text)
    ip_address = db.Column(db.String(50))
    device_info = db.Column(db.String(255))

class Admin(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    username = db.Column(db.String(50), unique=True, nullable=False)
    password_hash = db.Column(db.String(200), nullable=False)
    email = db.Column(db.String(100))
    last_login = db.Column(db.DateTime)

    def set_password(self, password):
        self.password_hash = generate_password_hash(password)
        
    def check_password(self, password):
        return check_password_hash(self.password_hash, password)

# Function to create default admin account
def create_default_admin():
    if Admin.query.count() == 0:
        default_admin = Admin(username='admin')
        default_admin.set_password('admin123')
        default_admin.email = 'admin@photostudio.com'
        db.session.add(default_admin)
        db.session.commit()
        print("Created default admin account: 'admin' / 'admin123'")

# Authentication decorator
def login_required(f):
    @wraps(f)
    def decorated_function(*args, **kwargs):
        if not session.get('admin_id'):
            flash('Пожалуйста, войдите для доступа к этой странице', 'warning')
            return redirect(url_for('login'))
        return f(*args, **kwargs)
    return decorated_function

# API Routes
@app.route('/api/users', methods=['GET'])
def get_users():
    users = User.query.all()
    return jsonify([{
        'id': user.id,
        'username': user.username,
        'email': user.email,
        'device_id': user.device_id,
        'last_login': user.last_login.isoformat() if user.last_login else None,
        'created_at': user.created_at.isoformat()
    } for user in users])

@app.route('/api/users', methods=['POST'])
def create_user():
    data = request.json
    
    # Check if username already exists
    existing_user = User.query.filter_by(username=data['username']).first()
    if existing_user:
        return jsonify({'error': 'Username already exists'}), 400
        
    new_user = User(
        username=data['username'],
        email=data.get('email'),
        device_id=data.get('device_id', str(uuid.uuid4()))
    )
    db.session.add(new_user)
    db.session.commit()
    
    # Log user creation activity
    log_activity(new_user.id, 'user_created', 'New user account created')
    
    return jsonify({
        'id': new_user.id, 
        'username': new_user.username,
        'device_id': new_user.device_id,
        'message': 'User created successfully'
    })

@app.route('/api/users/<int:user_id>', methods=['GET'])
def get_user(user_id):
    user = User.query.get_or_404(user_id)
    return jsonify({
        'id': user.id,
        'username': user.username,
        'email': user.email,
        'device_id': user.device_id,
        'last_login': user.last_login.isoformat() if user.last_login else None,
        'created_at': user.created_at.isoformat()
    })

@app.route('/api/users/<int:user_id>', methods=['PUT'])
def update_user(user_id):
    user = User.query.get_or_404(user_id)
    data = request.json
    
    if 'username' in data and data['username'] != user.username:
        existing_user = User.query.filter_by(username=data['username']).first()
        if existing_user:
            return jsonify({'error': 'Username already exists'}), 400
        user.username = data['username']
        
    if 'email' in data:
        user.email = data['email']
        
    if 'device_id' in data:
        user.device_id = data['device_id']
    
    db.session.commit()
    log_activity(user_id, 'user_updated', 'User profile updated')
    
    return jsonify({'message': 'User updated successfully'})

@app.route('/api/users/<int:user_id>/login', methods=['POST'])
def user_login(user_id):
    user = User.query.get_or_404(user_id)
    user.last_login = datetime.utcnow()
    db.session.commit()
    
    # Log login activity
    device_info = request.json.get('device_info', '')
    ip_address = request.remote_addr
    log_activity(user_id, 'user_login', 'User logged in', device_info, ip_address)
    
    return jsonify({'message': 'Login recorded successfully'})

@app.route('/api/activities', methods=['GET'])
def get_activities():
    activities = UserActivity.query.order_by(UserActivity.timestamp.desc()).all()
    return jsonify([{
        'id': activity.id,
        'user_id': activity.user_id,
        'username': activity.user.username,
        'action': activity.action,
        'timestamp': activity.timestamp.isoformat(),
        'details': activity.details,
        'ip_address': activity.ip_address,
        'device_info': activity.device_info
    } for activity in activities])

@app.route('/api/activities/user/<int:user_id>', methods=['GET'])
def get_user_activities(user_id):
    user = User.query.get_or_404(user_id)
    activities = UserActivity.query.filter_by(user_id=user_id).order_by(UserActivity.timestamp.desc()).all()
    return jsonify([{
        'id': activity.id,
        'action': activity.action,
        'timestamp': activity.timestamp.isoformat(),
        'details': activity.details,
        'ip_address': activity.ip_address,
        'device_info': activity.device_info
    } for activity in activities])

@app.route('/api/system/info', methods=['GET'])
def get_system_info():
    system_info = {
        'platform': platform.system(),
        'platform_release': platform.release(),
        'platform_version': platform.version(),
        'architecture': platform.machine(),
        'processor': platform.processor(),
        'hostname': platform.node(),
        'python_version': platform.python_version(),
        'memory': {
            'total': psutil.virtual_memory().total,
            'available': psutil.virtual_memory().available,
            'percent_used': psutil.virtual_memory().percent
        },
        'disk': {
            'total': psutil.disk_usage('/').total,
            'used': psutil.disk_usage('/').used,
            'free': psutil.disk_usage('/').free,
            'percent_used': psutil.disk_usage('/').percent
        },
        'cpu': {
            'physical_cores': psutil.cpu_count(logical=False),
            'total_cores': psutil.cpu_count(logical=True),
            'current_frequency': psutil.cpu_freq().current if psutil.cpu_freq() else None,
            'percent_usage': psutil.cpu_percent(interval=1)
        },
        'server_time': datetime.utcnow().isoformat()
    }
    return jsonify(system_info)

@app.route('/api/statistics', methods=['GET'])
def get_statistics():
    total_users = User.query.count()
    active_users = User.query.filter(User.last_login > datetime.utcnow().replace(hour=0, minute=0, second=0, microsecond=0)).count()
    total_activities = UserActivity.query.count()
    
    # Get activities by type
    activities_by_type = {}
    for activity in UserActivity.query.all():
        if activity.action in activities_by_type:
            activities_by_type[activity.action] += 1
        else:
            activities_by_type[activity.action] = 1
    
    # Get recent activities
    recent_activities = UserActivity.query.order_by(UserActivity.timestamp.desc()).limit(10).all()
    recent_activities_data = [{
        'id': activity.id,
        'user_id': activity.user_id,
        'username': activity.user.username,
        'action': activity.action,
        'timestamp': activity.timestamp.isoformat(),
        'details': activity.details
    } for activity in recent_activities]
    
    statistics = {
        'total_users': total_users,
        'active_users_today': active_users,
        'total_activities': total_activities,
        'activities_by_type': activities_by_type,
        'recent_activities': recent_activities_data
    }
    
    return jsonify(statistics)

# Admin Web Interface Routes
@app.route('/')
def index():
    return render_template('index.html')

@app.route('/login', methods=['GET', 'POST'])
def login():
    if request.method == 'POST':
        username = request.form.get('username')
        password = request.form.get('password')
        
        admin = Admin.query.filter_by(username=username).first()
        
        if admin and admin.check_password(password):
            session['admin_id'] = admin.id
            admin.last_login = datetime.utcnow()
            db.session.commit()
            flash('Вы успешно вошли в систему', 'success')
            return redirect(url_for('admin_dashboard'))
        else:
            flash('Неверное имя пользователя или пароль', 'danger')
    
    return render_template('login.html')

@app.route('/logout')
def logout():
    session.pop('admin_id', None)
    flash('Вы вышли из системы', 'info')
    return redirect(url_for('login'))

@app.route('/admin')
@login_required
def admin_dashboard():
    users_count = User.query.count()
    activities_count = UserActivity.query.count()
    active_users_count = User.query.filter(User.last_login > datetime.utcnow().replace(hour=0, minute=0, second=0, microsecond=0)).count()
    now = datetime.now()
    
    # Get recent activities
    recent_activities = UserActivity.query.order_by(UserActivity.timestamp.desc()).limit(10).all()
    
    return render_template('admin/dashboard.html', 
                          users_count=users_count, 
                          activities_count=activities_count, 
                          active_users_count=active_users_count,
                          recent_activities=recent_activities,
                          now=now)

@app.route('/admin/profile', methods=['GET', 'POST'])
@login_required
def admin_profile():
    admin = Admin.query.get(session['admin_id'])
    
    if request.method == 'POST':
        action = request.form.get('action')
        
        if action == 'update_profile':
            username = request.form.get('username')
            email = request.form.get('email')
            
            # Проверка, не занято ли имя пользователя
            if username != admin.username and Admin.query.filter_by(username=username).first():
                flash('Это имя пользователя уже занято', 'danger')
            else:
                admin.username = username
                admin.email = email
                db.session.commit()
                flash('Профиль успешно обновлен', 'success')
                
        elif action == 'change_password':
            current_password = request.form.get('current_password')
            new_password = request.form.get('new_password')
            confirm_password = request.form.get('confirm_password')
            
            if not admin.check_password(current_password):
                flash('Текущий пароль неверный', 'danger')
            elif new_password != confirm_password:
                flash('Новые пароли не совпадают', 'danger')
            else:
                admin.set_password(new_password)
                db.session.commit()
                flash('Пароль успешно изменен', 'success')
                
    return render_template('admin/profile.html', admin=admin)

@app.route('/admin/users')
@login_required
def admin_users():
    users = User.query.all()
    return render_template('admin/users.html', users=users)

@app.route('/admin/activities')
@login_required
def admin_activities():
    activities = UserActivity.query.order_by(UserActivity.timestamp.desc()).all()
    return render_template('admin/activities.html', activities=activities)

@app.route('/admin/system')
@login_required
def admin_system():
    system_info = {
        'platform': platform.system(),
        'platform_release': platform.release(),
        'platform_version': platform.version(),
        'architecture': platform.machine(),
        'processor': platform.processor(),
        'hostname': platform.node(),
        'python_version': platform.python_version(),
        'memory': {
            'total': psutil.virtual_memory().total,
            'available': psutil.virtual_memory().available,
            'percent_used': psutil.virtual_memory().percent
        },
        'disk': {
            'total': psutil.disk_usage('/').total,
            'used': psutil.disk_usage('/').used,
            'free': psutil.disk_usage('/').free,
            'percent_used': psutil.disk_usage('/').percent
        },
        'cpu': {
            'physical_cores': psutil.cpu_count(logical=False),
            'total_cores': psutil.cpu_count(logical=True),
            'current_frequency': psutil.cpu_freq().current if psutil.cpu_freq() else None,
            'percent_usage': psutil.cpu_percent(interval=1)
        }
    }
    return render_template('admin/system.html', system_info=system_info)

@app.route('/initialize-admin', methods=['GET'])
def initialize_admin():
    if Admin.query.count() == 0:
        admin = Admin(username='admin')
        admin.set_password('admin123')
        db.session.add(admin)
        db.session.commit()
        return "Admin created: login 'admin', password 'admin123'"
    return "Admin already exists"

# Helper function to log user activities
def log_activity(user_id, action, details=None, device_info=None, ip_address=None):
    new_activity = UserActivity(
        user_id=user_id,
        action=action,
        details=details,
        device_info=device_info,
        ip_address=ip_address
    )
    db.session.add(new_activity)
    db.session.commit()
    return new_activity

if __name__ == '__main__':
    with app.app_context():
        db.create_all()
        create_default_admin()
    app.run(debug=True, host='0.0.0.0', port=5000)