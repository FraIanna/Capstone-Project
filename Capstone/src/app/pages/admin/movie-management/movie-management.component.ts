import { Component, TemplateRef, ViewChild } from '@angular/core';
import { MovieService } from '../../../services/movie.service';
import { iMovie } from '../../../models/i-movie';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { iHall } from '../../../models/i-hall';
import { HallService } from '../../../services/hall.service';
import { JsonPipe } from '@angular/common';

@Component({
  selector: 'app-movie-management',
  templateUrl: './movie-management.component.html',
  styleUrl: './movie-management.component.scss',
})

//WORKING IN PROGRESS
export class MovieManagementComponent {
  @ViewChild('movieModal') movieModal!: TemplateRef<any>;

  halls: iHall[] = [];
  movies: iMovie[] = [];
  selectedMovie!: iMovie | null;
  movieForm!: FormGroup;
  create!: FormGroup;
  selectedImageFile!: File;

  constructor(
    private movieSvc: MovieService,
    private hallSvc: HallService,
    private modalService: NgbModal,
    private fb: FormBuilder
  ) {}

  ngOnInit() {
    this.hallSvc.getAllHalls().subscribe((halls: iHall[]) => {
      this.halls = halls;
    });

    this.movieSvc.getAllMovies().subscribe({
      next: (movies: iMovie[]) => {
        this.movies = movies;
      },
      error: (error) => {
        console.error('Error retrieving movies:', error);
      },
    });

    this.create = this.fb.group({
      title: this.fb.control(null, [Validators.required]),
      description: this.fb.control(null, [Validators.required]),
      releaseDate: this.fb.control(null, [Validators.required]),
      category: this.fb.control(null, [Validators.required]),
      runtime: this.fb.control(null, [Validators.required, Validators.min(1)]),
      imagePath: this.fb.control(null, [Validators.required]),
      trailerUrl: this.fb.control(null, [
        Validators.required,
        Validators.pattern('https?://.+'),
      ]),
      casts: this.fb.control(null, [Validators.required]),
      showtimes: this.fb.group({
        start: this.fb.control(null, [Validators.required]),
        end: this.fb.control(null, [Validators.required]),
      }),
      hallId: this.fb.control(null, [Validators.required]),
    });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedImageFile = input.files[0];
    }
  }

  createMovie(): void {
    if (this.create.invalid) {
      return;
    }

    const formValue = this.create.value;
    const movieData = {
      ...formValue,
      showtimes: ([] = [
        {
          start: formValue.showtimes.start,
          end: formValue.showtimes.end,
        },
      ]),
      imagePath: formValue.imagePath,
    };
    const formData = new FormData();
    formData.append('title', movieData.title);
    formData.append('description', JSON.stringify(movieData.description));
    formData.append('releaseDate', JSON.stringify(movieData.releaseDate));
    formData.append('category', JSON.stringify(movieData.category));
    formData.append('runtime', JSON.stringify(movieData.runtime));
    formData.append('trailerUrl', JSON.stringify(movieData.trailerUrl));
    formData.append('casts', JSON.stringify(movieData.casts));
    formData.append('hallId', JSON.stringify(movieData.hallId));
    formData.append('showtimes', JSON.stringify(movieData.showtimes));
    formData.append('start', this.create.get('showtimes.start')?.value);
    formData.append('end', this.create.get('showtimes.end')?.value);

    if (this.selectedImageFile) {
      formData.append('image', this.selectedImageFile);
    }

    for (let pair of (formData as any).entries()) {
      console.log(`${pair[0]}: ${pair[1]}`);
    }

    this.movieSvc.create(formData).subscribe({
      next: (response) => {
        console.log('Movie created:', response);
        this.modalService.dismissAll();
        this.ngOnInit();
      },
      error: (error) => {
        console.error('Error creating movie:', error);
      },
    });
  }

  deleteMovie(movie: iMovie): void {
    if (movie && movie.id != null) {
      this.movieSvc.delete(movie.id).subscribe({
        next: () => {
          this.movies = this.movies.filter((m) => m.id !== movie.id);
          console.log(`${movie.title} eliminato con successo.`);
        },
        error: (error: any) => {
          console.error("Errore durante l'eliminazione della sala:", error);
        },
      });
    } else {
      console.warn('Sala non valida o ID mancante.');
    }
  }

  isTouchedAndInvalid(fieldName: string) {
    const field = this.create.get(fieldName) || this.movieForm.get(fieldName);
    return field?.invalid && field?.touched;
  }

  openModal(movie?: iMovie): void {
    this.selectedMovie = movie || null;
    this.initForm();
    this.modalService.open(this.movieModal);
    console.log(this.selectedMovie);
  }

  initForm(): void {
    this.movieForm = this.fb.group({
      id: [this.selectedMovie?.id || null],
      title: [this.selectedMovie?.title || null, [Validators.minLength(3)]],
      description: [this.selectedMovie?.description || null],
      category: [this.selectedMovie?.category || null, [Validators.min(1)]],
      runtime: [
        this.selectedMovie?.runtime || null,
        [Validators.min(1), Validators.max(300)],
      ],
      imagePath: [this.selectedMovie?.imagePath || null],
      trailerUrl: [
        this.selectedMovie?.trailerUrl || null,
        [Validators.pattern('https?://.+')],
      ],
      casts: [this.selectedMovie?.casts || null, [Validators.minLength(3)]],
      showtimes: [this.selectedMovie?.showtimes || null],
      hallId: [this.selectedMovie?.hallId || null],
    });
  }

  submitModal(modal: any): void {
    if (this.movieForm.invalid) {
      return;
    }

    const formData = new FormData();
    formData.append('id', this.movieForm.value.id);
    formData.append('title', this.movieForm.value.title);
    formData.append('description', this.movieForm.value.description);
    formData.append('category', this.movieForm.value.category);
    formData.append('runtime', this.movieForm.value.runtime);
    formData.append('releaseDate', this.movieForm.value.releaseDate);
    formData.append('trailerUrl', this.movieForm.value.trailerUrl);
    formData.append('casts', this.movieForm.value.casts);
    formData.append(
      'showtimes',
      JSON.stringify(this.movieForm.value.showtimes)
    );
    formData.append('hallId', this.movieForm.value.hallId);

    if (this.selectedImageFile) {
      formData.append('image', this.selectedImageFile);
    }

    if (this.movieForm.value.id != null) {
      this.movieSvc.update(this.movieForm.value.id, formData).subscribe({
        next: () => {
          console.log('Film aggiornato con successo');
          this.movieSvc.getAllMovies().subscribe({
            next: (movies: iMovie[]) => {
              this.movies = movies;
            },
            error: (error: any) => {
              console.error('Error retrieving movies:', error);
            },
          });
          this.modalService.dismissAll();
        },
        error: (error) => {
          console.error("Errore durante l'aggiornamento del film", error);
          console.log(this.movieForm.value);
        },
      });
    }
  }
}
