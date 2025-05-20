from flask import Flask, request, jsonify
from flask_sqlalchemy import SQLAlchemy
from flask_cors import CORS
from datetime import datetime

app = Flask(__name__)
CORS(app)

# Database configuration
app.config['SQLALCHEMY_DATABASE_URI'] = 'sqlite:///photostudio.db'
app.config['SQLALCHEMY_TRACK_MODIFICATIONS'] = False
db = SQLAlchemy(app)

# Models
class Client(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    name = db.Column(db.String(100), nullable=False)
    phone = db.Column(db.String(20), nullable=False)
    email = db.Column(db.String(100))
    sessions = db.relationship('Session', backref='client', lazy=True)

class Session(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    date = db.Column(db.DateTime, nullable=False)
    duration = db.Column(db.Integer, nullable=False)  # Duration in minutes
    price = db.Column(db.Float, nullable=False)
    status = db.Column(db.String(20), nullable=False)  # 'scheduled', 'completed', 'cancelled'
    client_id = db.Column(db.Integer, db.ForeignKey('client.id'), nullable=False)
    photos = db.relationship('Photo', backref='session', lazy=True)

class Photo(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    filename = db.Column(db.String(255), nullable=False)
    path = db.Column(db.String(255), nullable=False)
    session_id = db.Column(db.Integer, db.ForeignKey('session.id'), nullable=False)
    created_at = db.Column(db.DateTime, default=datetime.utcnow)

# Routes
@app.route('/api/clients', methods=['GET'])
def get_clients():
    clients = Client.query.all()
    return jsonify([{
        'id': client.id,
        'name': client.name,
        'phone': client.phone,
        'email': client.email
    } for client in clients])

@app.route('/api/clients', methods=['POST'])
def create_client():
    data = request.json
    new_client = Client(
        name=data['name'],
        phone=data['phone'],
        email=data.get('email')
    )
    db.session.add(new_client)
    db.session.commit()
    return jsonify({'id': new_client.id, 'message': 'Client created successfully'})

@app.route('/api/sessions', methods=['GET'])
def get_sessions():
    sessions = Session.query.all()
    return jsonify([{
        'id': session.id,
        'date': session.date.isoformat(),
        'duration': session.duration,
        'price': session.price,
        'status': session.status,
        'client_id': session.client_id
    } for session in sessions])

@app.route('/api/sessions', methods=['POST'])
def create_session():
    data = request.json
    new_session = Session(
        date=datetime.fromisoformat(data['date']),
        duration=data['duration'],
        price=data['price'],
        status=data['status'],
        client_id=data['client_id']
    )
    db.session.add(new_session)
    db.session.commit()
    return jsonify({'id': new_session.id, 'message': 'Session created successfully'})

@app.route('/api/photos', methods=['GET'])
def get_photos():
    photos = Photo.query.all()
    return jsonify([{
        'id': photo.id,
        'filename': photo.filename,
        'path': photo.path,
        'session_id': photo.session_id,
        'created_at': photo.created_at.isoformat()
    } for photo in photos])

@app.route('/api/photos', methods=['POST'])
def create_photo():
    data = request.json
    new_photo = Photo(
        filename=data['filename'],
        path=data['path'],
        session_id=data['session_id']
    )
    db.session.add(new_photo)
    db.session.commit()
    return jsonify({'id': new_photo.id, 'message': 'Photo added successfully'})

if __name__ == '__main__':
    with app.app_context():
        db.create_all()
    app.run(debug=True, port=5000) 