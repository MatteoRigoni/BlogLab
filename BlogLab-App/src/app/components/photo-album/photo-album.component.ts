import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { Photo } from 'src/app/models/photos/photo.model';
import { PhotoService } from 'src/app/services/photo.service';

@Component({
  selector: 'app-photo-album',
  templateUrl: './photo-album.component.html',
  styleUrls: ['./photo-album.component.css']
})
export class PhotoAlbumComponent implements OnInit {

  @ViewChild('photoForm') photoForm: NgForm;
  @ViewChild('photoUploadElement') photoUploadElement: ElementRef;

  photos: Photo[] = [];
  photoFile: any;
  newPhotoDescription: string;

  constructor(
    private photoService: PhotoService,
    private toastr: ToastrService
  ) { }

  ngOnInit(): void {
    this.photoService.getByApplicationUserId().subscribe(userPhotos => {
      this.photos = userPhotos;
    });
  }

  confirmDelete(photo: Photo) {
    photo.deleteConfirm = true;
  }

  cancelDeleteConfirm(photo: Photo) {
    photo.deleteConfirm = false;
  }

  deleteConfirmed(photo: Photo) {
    this.photoService.delete(photo.photoId).subscribe(() => {
      let index = 0;

      for (let i=0; i<this.photos.length; i++) {
        if (this.photos[i].photoId === photo.photoId) {
          index = i;
        }
      }

      if (index > -1) {
        this.photos.splice(index, 1);
      }

      this.toastr.info("Photo deleted.");
    });
  }

  onFileChange(event: Event) {
    const element = event.target as HTMLInputElement;
    if (element.files.length > 0) {
      let fileList: FileList | null = element.files;
      if (fileList) {
        const file = fileList[0];
        this.photoFile = file;
      }
    }
  }

  onSubmit() {

    const formData = new FormData();
    formData.append('file', this.photoFile, this.newPhotoDescription);

    this.photoService.create(formData).subscribe(createdPhoto => {

      this.photoForm.reset();
      this.photoUploadElement.nativeElement.value = '';

      this.toastr.info("Photo uploaded");
      this.photos.unshift(createdPhoto);

    });
  }
}
function ViewChildChild(arg0: string) {
  throw new Error('Function not implemented.');
}

