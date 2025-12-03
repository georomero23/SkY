import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MiPaginadorComponent } from './mi-paginador.component';

describe('MiPaginadorComponent', () => {
  let component: MiPaginadorComponent;
  let fixture: ComponentFixture<MiPaginadorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MiPaginadorComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MiPaginadorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
